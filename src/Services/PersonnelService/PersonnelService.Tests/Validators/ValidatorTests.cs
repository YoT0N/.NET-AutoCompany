using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.DeletePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnelStatus;
using PersonnelService.Application.TodoExaminations.Commands.CreateExamination;
using PersonnelService.Domain.Interfaces;
using Xunit;

namespace PersonnelService.Tests.Validators;

public class CreatePersonnelCommandValidatorTests
{
    private readonly Mock<IPersonnelRepository> _repoMock = new();
    private CreatePersonnelCommandValidator CreateValidator() => new(_repoMock.Object);

    private static CreatePersonnelCommand ValidCommand() => new()
    {
        FullName = "Іван Тестенко",
        BirthDate = DateTime.UtcNow.AddYears(-25),
        Position = "Driver",
        Phone = "+380501234567",
        Email = "ivan@example.com",
        Address = "Київ, вул. Тестова, 1"
    };

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        var result = await CreateValidator().TestValidateAsync(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")]  // too short
    public async Task Validate_InvalidFullName_FailsValidation(string fullName)
    {
        var command = ValidCommand();
        command.FullName = fullName;

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }


    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public async Task Validate_InvalidEmail_FailsValidation(string email)
    {
        var command = ValidCommand();
        command.Email = email;

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("123")]     // too short
    [InlineData("abc")]     // not digits
    public async Task Validate_InvalidPhone_FailsValidation(string phone)
    {
        var command = ValidCommand();
        command.Phone = phone;

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public async Task Validate_EmptyPosition_FailsValidation()
    {
        var command = ValidCommand();
        command.Position = "";

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Position);
    }
}

public class UpdatePersonnelStatusCommandValidatorTests
{
    private readonly Mock<IPersonnelRepository> _repoMock = new();

    private UpdatePersonnelStatusCommandValidator CreateValidator() => new(_repoMock.Object);

    [Theory]
    [InlineData("Active")]
    [InlineData("Inactive")]
    [InlineData("OnLeave")]
    [InlineData("Terminated")]
    public async Task Validate_ValidStatus_PassesValidation(string status)
    {
        var id = "507f1f77bcf86cd799439011";
        _repoMock.Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateValidator().TestValidateAsync(
            new UpdatePersonnelStatusCommand { Id = id, Status = status });

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Deleted")]
    [InlineData("")]
    public async Task Validate_InvalidStatus_FailsValidation(string status)
    {
        var id = "507f1f77bcf86cd799439011";
        _repoMock.Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateValidator().TestValidateAsync(
            new UpdatePersonnelStatusCommand { Id = id, Status = status });

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public async Task Validate_InvalidObjectIdFormat_FailsValidation()
    {
        var result = await CreateValidator().TestValidateAsync(
            new UpdatePersonnelStatusCommand { Id = "not-a-valid-id", Status = "Active" });

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Invalid Id format.");
    }
}

public class DeletePersonnelCommandValidatorTests
{
    private readonly Mock<IPersonnelRepository> _repoMock = new();

    private DeletePersonnelCommandValidator CreateValidator() => new(_repoMock.Object);

    [Fact]
    public async Task Validate_ExistingPersonnel_PassesValidation()
    {
        var id = "507f1f77bcf86cd799439011";
        _repoMock.Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateValidator().TestValidateAsync(new DeletePersonnelCommand { Id = id });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_NonExistingPersonnel_FailsValidation()
    {
        var id = "507f1f77bcf86cd799439011";
        _repoMock.Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateValidator().TestValidateAsync(new DeletePersonnelCommand { Id = id });

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Personnel not found.");
    }

    [Fact]
    public async Task Validate_EmptyId_FailsValidation()
    {
        var result = await CreateValidator().TestValidateAsync(new DeletePersonnelCommand { Id = "" });

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

public class CreateExaminationCommandValidatorTests
{
    private readonly Mock<IPersonnelRepository> _personnelRepoMock = new();

    private CreateExaminationCommandValidator CreateValidator() => new(_personnelRepoMock.Object);

    private static CreateExaminationCommand ValidCommand() => new()
    {
        PersonnelId = 1,
        ExamDate = DateTime.UtcNow.AddDays(-1),
        Result = "Passed",
        DoctorName = "Dr. Іваненко",
        Height = 178,
        Weight = 75,
        BloodPressure = "120/80",
        Vision = "1.0/1.0"
    };

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        _personnelRepoMock
            .Setup(r => r.ExistsByPersonnelIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await CreateValidator().TestValidateAsync(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("UnknownResult")]
    [InlineData("")]
    public async Task Validate_InvalidResult_FailsValidation(string resultValue)
    {
        _personnelRepoMock
            .Setup(r => r.ExistsByPersonnelIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = ValidCommand();
        command.Result = resultValue;

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Result);
    }

    [Theory]
    [InlineData("120-80")]    // wrong format
    [InlineData("bp")]        // non-numeric
    public async Task Validate_InvalidBloodPressureFormat_FailsValidation(string bp)
    {
        _personnelRepoMock
            .Setup(r => r.ExistsByPersonnelIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = ValidCommand();
        command.BloodPressure = bp;

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BloodPressure);
    }

    [Fact]
    public async Task Validate_FutureExamDate_FailsValidation()
    {
        _personnelRepoMock
            .Setup(r => r.ExistsByPersonnelIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = ValidCommand();
        command.ExamDate = DateTime.UtcNow.AddDays(10);

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExamDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(300)]
    public async Task Validate_InvalidHeight_FailsValidation(int height)
    {
        _personnelRepoMock
            .Setup(r => r.ExistsByPersonnelIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = ValidCommand();
        command.Height = height;

        var result = await CreateValidator().TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Height);
    }
}