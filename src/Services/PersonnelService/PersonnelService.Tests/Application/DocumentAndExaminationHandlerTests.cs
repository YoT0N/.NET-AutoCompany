using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Application.TodoDocuments.Commands.CreateDocument;
using PersonnelService.Application.TodoDocuments.Commands.DeleteDocument;
using PersonnelService.Application.TodoDocuments.Queries.GetDocumentsByPersonnel;
using PersonnelService.Application.TodoDocuments.Queries.GetExpiringDocuments;
using PersonnelService.Application.TodoExaminations.Commands.CreateExamination;
using PersonnelService.Application.TodoExaminations.Queries.GetExaminationsByPersonnel;
using PersonnelService.Application.TodoExaminations.Queries.GetLatestExamination;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using Xunit;

namespace PersonnelService.Tests.Application;

public class DocumentHandlerTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private static PersonnelDocument CreateDocument(int personnelId = 1)
    {
        var doc = new PersonnelDocument(
            personnelId: personnelId,
            docType: "DriverLicense",
            fileName: "license.pdf",
            fileSize: 100_000,
            mimeType: "application/pdf",
            issuedOn: DateTime.UtcNow.AddYears(-1),
            validUntil: DateTime.UtcNow.AddYears(5)
        );
        doc.Id = "507f1f77bcf86cd799439011";
        return doc;
    }

    // ─── CreateDocumentCommandHandler ──────────────────────────────────────────

    public class CreateDocumentCommandHandlerTests
    {
        private readonly Mock<IDocumentRepository> _docRepoMock = new();
        private readonly Mock<IPersonnelRepository> _personnelRepoMock = new();
        private readonly Mock<ILogger<CreateDocumentCommandHandler>> _loggerMock = new();

        private CreateDocumentCommandHandler CreateHandler() =>
            new(_docRepoMock.Object, _personnelRepoMock.Object, _loggerMock.Object);

       

        [Fact]
        public async Task Handle_PersonnelNotExists_ThrowsNotFoundException()
        {
            _personnelRepoMock
                .Setup(r => r.ExistsByPersonnelIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new CreateDocumentCommand
            {
                PersonnelId = 99,
                DocType = "DriverLicense",
                FileName = "license.pdf",
                FileSize = 100_000,
                MimeType = "application/pdf"
            };

            var act = () => CreateHandler().Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Personnel*");

            _docRepoMock.Verify(r => r.AddAsync(It.IsAny<PersonnelDocument>()), Times.Never);
        }
    }

    // ─── DeleteDocumentCommandHandler ───────────────────────────────────────────

    public class DeleteDocumentCommandHandlerTests
    {
        private readonly Mock<IDocumentRepository> _docRepoMock = new();
        private readonly Mock<ILogger<DeleteDocumentCommandHandler>> _loggerMock = new();

        private DeleteDocumentCommandHandler CreateHandler() =>
            new(_docRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_ExistingDocument_DeletesSuccessfully()
        {
            var id = "507f1f77bcf86cd799439011";
            _docRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(CreateDocument());
            _docRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await CreateHandler().Handle(new DeleteDocumentCommand { Id = id }, CancellationToken.None);

            result.Should().Contain(id);
            _docRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingDocument_ThrowsNotFoundException()
        {
            var id = "507f1f77bcf86cd799439011";
            _docRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((PersonnelDocument?)null);

            var act = () => CreateHandler().Handle(new DeleteDocumentCommand { Id = id }, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _docRepoMock.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }

    // ─── GetDocumentsByPersonnelQueryHandler ───────────────────────────────────

    public class GetDocumentsByPersonnelQueryHandlerTests
    {
        private readonly Mock<IDocumentRepository> _docRepoMock = new();
        private readonly Mock<ILogger<GetDocumentsByPersonnelQueryHandler>> _loggerMock = new();
        private readonly IMapper _mapper = CreateMapper();

        private GetDocumentsByPersonnelQueryHandler CreateHandler() =>
            new(_docRepoMock.Object, _mapper, _loggerMock.Object);

        [Fact]
        public async Task Handle_PersonnelWithDocuments_ReturnsDocuments()
        {
            var docs = new List<PersonnelDocument> { CreateDocument(1), CreateDocument(1) };
            _docRepoMock.Setup(r => r.GetByPersonnelIdAsync(1)).ReturnsAsync(docs.AsReadOnly());

            var result = await CreateHandler().Handle(
                new GetDocumentsByPersonnelQuery(1), CancellationToken.None);

            result.Should().HaveCount(2);
            result.All(d => d.PersonnelId == 1).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_PersonnelWithNoDocuments_ReturnsEmptyList()
        {
            _docRepoMock
                .Setup(r => r.GetByPersonnelIdAsync(1))
                .ReturnsAsync(new List<PersonnelDocument>().AsReadOnly());

            var result = await CreateHandler().Handle(
                new GetDocumentsByPersonnelQuery(1), CancellationToken.None);

            result.Should().BeEmpty();
        }
    }

    // ─── GetExpiringDocumentsQueryHandler ──────────────────────────────────────

    public class GetExpiringDocumentsQueryHandlerTests
    {
        private readonly Mock<IDocumentRepository> _docRepoMock = new();
        private readonly Mock<ILogger<GetExpiringDocumentsQueryHandler>> _loggerMock = new();
        private readonly IMapper _mapper = CreateMapper();

        private GetExpiringDocumentsQueryHandler CreateHandler() =>
            new(_docRepoMock.Object, _mapper, _loggerMock.Object);

        [Fact]
        public async Task Handle_WithExpiringDocs_ReturnsCorrectCount()
        {
            var expiring = new List<PersonnelDocument> { CreateDocument() };
            _docRepoMock.Setup(r => r.GetExpiringDocumentsAsync(30)).ReturnsAsync(expiring.AsReadOnly());

            var result = await CreateHandler().Handle(
                new GetExpiringDocumentsQuery(30), CancellationToken.None);

            result.Should().HaveCount(1);
            _docRepoMock.Verify(r => r.GetExpiringDocumentsAsync(30), Times.Once);
        }
    }
}

public class ExaminationHandlerTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private static PhysicalExamination CreateExamination(int personnelId = 1)
    {
        var exam = new PhysicalExamination(
            personnelId: personnelId,
            examDate: DateTime.UtcNow.AddMonths(-1),
            result: "Passed",
            doctorName: "Dr. Тест",
            metrics: new ExaminationMetricsVO(175, 70, "120/80", "1.0/1.0")
        );
        exam.Id = "507f1f77bcf86cd799439011";
        return exam;
    }

    // ─── CreateExaminationCommandHandler ───────────────────────────────────────

    public class CreateExaminationCommandHandlerTests
    {
        private readonly Mock<IExaminationRepository> _examRepoMock = new();
        private readonly Mock<IPersonnelRepository> _personnelRepoMock = new();
        private readonly Mock<ILogger<CreateExaminationCommandHandler>> _loggerMock = new();

        private CreateExaminationCommandHandler CreateHandler() =>
            new(_examRepoMock.Object, _personnelRepoMock.Object, _loggerMock.Object);

        

        [Fact]
        public async Task Handle_PersonnelNotExists_ThrowsNotFoundException()
        {
            _personnelRepoMock
                .Setup(r => r.ExistsByPersonnelIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var act = () => CreateHandler().Handle(
                new CreateExaminationCommand { PersonnelId = 99, Result = "Passed", DoctorName = "Dr." },
                CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Personnel*");
        }
    }

    // ─── GetLatestExaminationQueryHandler ──────────────────────────────────────

    public class GetLatestExaminationQueryHandlerTests
    {
        private readonly Mock<IExaminationRepository> _examRepoMock = new();
        private readonly Mock<ILogger<GetLatestExaminationQueryHandler>> _loggerMock = new();
        private readonly IMapper _mapper = CreateMapper();

        private GetLatestExaminationQueryHandler CreateHandler() =>
            new(_examRepoMock.Object, _mapper, _loggerMock.Object);

        [Fact]
        public async Task Handle_ExaminationExists_ReturnsDto()
        {
            _examRepoMock
                .Setup(r => r.GetLatestByPersonnelIdAsync(1))
                .ReturnsAsync(CreateExamination());

            var result = await CreateHandler().Handle(
                new GetLatestExaminationQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result!.Result.Should().Be("Passed");
        }

        [Fact]
        public async Task Handle_NoExaminationFound_ReturnsNull()
        {
            _examRepoMock
                .Setup(r => r.GetLatestByPersonnelIdAsync(99))
                .ReturnsAsync((PhysicalExamination?)null);

            var result = await CreateHandler().Handle(
                new GetLatestExaminationQuery(99), CancellationToken.None);

            result.Should().BeNull();
        }
    }

    // ─── GetExaminationsByPersonnelQueryHandler ─────────────────────────────────

    public class GetExaminationsByPersonnelQueryHandlerTests
    {
        private readonly Mock<IExaminationRepository> _examRepoMock = new();
        private readonly Mock<ILogger<GetExaminationsByPersonnelQueryHandler>> _loggerMock = new();
        private readonly IMapper _mapper = CreateMapper();

        private GetExaminationsByPersonnelQueryHandler CreateHandler() =>
            new(_examRepoMock.Object, _mapper, _loggerMock.Object);

        [Fact]
        public async Task Handle_ReturnsAllExaminationsForPersonnel()
        {
            var exams = new List<PhysicalExamination> { CreateExamination(1), CreateExamination(1) };
            _examRepoMock
                .Setup(r => r.GetByPersonnelIdAsync(1))
                .ReturnsAsync(exams.AsReadOnly());

            var result = await CreateHandler().Handle(
                new GetExaminationsByPersonnelQuery(1), CancellationToken.None);

            result.Should().HaveCount(2);
        }
    }
}