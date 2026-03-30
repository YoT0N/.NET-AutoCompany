using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.DeletePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnelStatus;
using PersonnelService.Application.TodoPersonnel.Queries.GetAllPersonnel;
using PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelById;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using Xunit;

namespace PersonnelService.Tests.Application;

public class PersonnelHandlerTests
{
    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private static Personnel CreateDomainPersonnel(string id = "507f1f77bcf86cd799439011")
    {
        var personnel = new Personnel(
            personnelId: 1,
            fullName: "Іван Тестенко",
            birthDate: DateTime.UtcNow.AddYears(-30),
            position: "Driver",
            contacts: new PersonnelContactsVO("+380501234567", "ivan@example.com", "Київ")
        );
        personnel.Id = id;
        return personnel;
    }

    // ─── CreatePersonnelCommandHandler ─────────────────────────────────────────

    public class CreatePersonnelCommandHandlerTests
    {
        private readonly Mock<IPersonnelRepository> _repoMock = new();
        private readonly Mock<ILogger<CreatePersonnelCommandHandler>> _loggerMock = new();

        private CreatePersonnelCommandHandler CreateHandler() =>
            new(_repoMock.Object, _loggerMock.Object);


        [Fact]
        public async Task Handle_ValidCommand_CallsGetNextPersonnelId()
        {
            _repoMock.Setup(r => r.GetNextPersonnelIdAsync()).ReturnsAsync(10);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Personnel>())).Returns(Task.CompletedTask);

            var command = new CreatePersonnelCommand
            {
                FullName = "Тест Тестенко",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                Position = "Conductor",
                Phone = "+380501234567",
                Email = "test@example.com",
                Address = "Київ"
            };

            await CreateHandler().Handle(command, CancellationToken.None);

            _repoMock.Verify(r => r.GetNextPersonnelIdAsync(), Times.Once);
        }
    }

    // ─── GetPersonnelByIdQueryHandler ──────────────────────────────────────────

    public class GetPersonnelByIdQueryHandlerTests
    {
        private readonly Mock<IPersonnelRepository> _repoMock = new();
        private readonly Mock<ILogger<GetPersonnelByIdQueryHandler>> _loggerMock = new();
        private readonly IMapper _mapper = CreateMapper();

        private GetPersonnelByIdQueryHandler CreateHandler() =>
            new(_repoMock.Object, _mapper, _loggerMock.Object);

        [Fact]
        public async Task Handle_ExistingId_ReturnsPersonnelDto()
        {
            var id = "507f1f77bcf86cd799439011";
            var personnel = CreateDomainPersonnel(id);
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(personnel);

            var result = await CreateHandler().Handle(new GetPersonnelByIdQuery(id), CancellationToken.None);

            result.Should().NotBeNull();
            result.FullName.Should().Be("Іван Тестенко");
            result.Position.Should().Be("Driver");
        }

        [Fact]
        public async Task Handle_NonExistingId_ThrowsNotFoundException()
        {
            var id = "507f1f77bcf86cd799439011";
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Personnel?)null);

            var act = () => CreateHandler().Handle(new GetPersonnelByIdQuery(id), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Personnel*");
        }
    }

    // ─── GetAllPersonnelQueryHandler ────────────────────────────────────────────

    public class GetAllPersonnelQueryHandlerTests
    {
        private readonly Mock<IPersonnelRepository> _repoMock = new();
        private readonly Mock<ILogger<GetAllPersonnelQueryHandler>> _loggerMock = new();
        private readonly IMapper _mapper = CreateMapper();

        private GetAllPersonnelQueryHandler CreateHandler() =>
            new(_repoMock.Object, _mapper, _loggerMock.Object);

        [Fact]
        public async Task Handle_ReturnsAllPersonnel()
        {
            var personnelList = new List<Personnel> { CreateDomainPersonnel(), CreateDomainPersonnel("507f1f77bcf86cd799439012") };
            _repoMock
                .Setup(r => r.SearchAsync(null, null, null, 0, 10))
                .ReturnsAsync(personnelList.AsReadOnly());

            var result = await CreateHandler().Handle(new GetAllPersonnelQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_EmptyCollection_ReturnsEmptyList()
        {
            _repoMock
                .Setup(r => r.SearchAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Personnel>().AsReadOnly());

            var result = await CreateHandler().Handle(new GetAllPersonnelQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WithFilters_PassesFiltersToRepository()
        {
            _repoMock
                .Setup(r => r.SearchAsync("Іван", "Driver", "Active", 0, 10))
                .ReturnsAsync(new List<Personnel>().AsReadOnly());

            var query = new GetAllPersonnelQuery { SearchText = "Іван", Position = "Driver", Status = "Active" };

            await CreateHandler().Handle(query, CancellationToken.None);

            _repoMock.Verify(r => r.SearchAsync("Іван", "Driver", "Active", 0, 10), Times.Once);
        }
    }

    // ─── DeletePersonnelCommandHandler ─────────────────────────────────────────

    public class DeletePersonnelCommandHandlerTests
    {
        private readonly Mock<IPersonnelRepository> _repoMock = new();
        private readonly Mock<ILogger<DeletePersonnelCommandHandler>> _loggerMock = new();

        private DeletePersonnelCommandHandler CreateHandler() =>
            new(_repoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_ExistingPersonnel_DeletesAndReturnsMessage()
        {
            var id = "507f1f77bcf86cd799439011";
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(CreateDomainPersonnel(id));
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await CreateHandler().Handle(new DeletePersonnelCommand { Id = id }, CancellationToken.None);

            result.Should().Contain(id);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingPersonnel_ThrowsNotFoundException()
        {
            var id = "507f1f77bcf86cd799439011";
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Personnel?)null);

            var act = () => CreateHandler().Handle(new DeletePersonnelCommand { Id = id }, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }

    // ─── UpdatePersonnelStatusCommandHandler ───────────────────────────────────

    public class UpdatePersonnelStatusCommandHandlerTests
    {
        private readonly Mock<IPersonnelRepository> _repoMock = new();
        private readonly Mock<ILogger<UpdatePersonnelStatusCommandHandler>> _loggerMock = new();

        private UpdatePersonnelStatusCommandHandler CreateHandler() =>
            new(_repoMock.Object, _loggerMock.Object);

        [Theory]
        [InlineData("Active")]
        [InlineData("Inactive")]
        [InlineData("OnLeave")]
        [InlineData("Terminated")]
        public async Task Handle_ValidStatus_UpdatesPersonnel(string status)
        {
            var id = "507f1f77bcf86cd799439011";
            var personnel = CreateDomainPersonnel(id);
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(personnel);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Personnel>())).Returns(Task.CompletedTask);

            var result = await CreateHandler().Handle(
                new UpdatePersonnelStatusCommand { Id = id, Status = status },
                CancellationToken.None);

            result.Should().Contain(status);
            _repoMock.Verify(r => r.UpdateAsync(It.Is<Personnel>(p => p.Status == status)), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingPersonnel_ThrowsNotFoundException()
        {
            var id = "507f1f77bcf86cd799439011";
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Personnel?)null);

            var act = () => CreateHandler().Handle(
                new UpdatePersonnelStatusCommand { Id = id, Status = "Active" },
                CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }

    // ─── UpdatePersonnelCommandHandler ─────────────────────────────────────────

    public class UpdatePersonnelCommandHandlerTests
    {
        private readonly Mock<IPersonnelRepository> _repoMock = new();
        private readonly Mock<ILogger<UpdatePersonnelCommandHandler>> _loggerMock = new();

        private UpdatePersonnelCommandHandler CreateHandler() =>
            new(_repoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_UpdateFullName_CallsUpdateAsync()
        {
            var id = "507f1f77bcf86cd799439011";
            var personnel = CreateDomainPersonnel(id);
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(personnel);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Personnel>())).Returns(Task.CompletedTask);

            await CreateHandler().Handle(
                new UpdatePersonnelCommand { Id = id, FullName = "Нове Ім'я" },
                CancellationToken.None);

            _repoMock.Verify(r => r.UpdateAsync(It.Is<Personnel>(p => p.FullName == "Нове Ім'я")), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingPersonnel_ThrowsNotFoundException()
        {
            var id = "507f1f77bcf86cd799439011";
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Personnel?)null);

            var act = () => CreateHandler().Handle(
                new UpdatePersonnelCommand { Id = id, FullName = "Тест" },
                CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}