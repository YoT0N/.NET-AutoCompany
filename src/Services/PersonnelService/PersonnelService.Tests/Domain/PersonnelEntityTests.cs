using FluentAssertions;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Exceptions;
using PersonnelService.Domain.ValueObjects;
using Xunit;

namespace PersonnelService.Tests.Domain;

public class PersonnelEntityTests
{
    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static PersonnelContactsVO ValidContacts() =>
        new("+380501234567", "test@example.com", "Київ, вул. Тестова, 1");

    private static Personnel CreatePersonnel(
        string fullName = "Іван Тестенко",
        DateTime? birthDate = null,
        string position = "Driver") =>
        new(
            personnelId: 1,
            fullName: fullName,
            birthDate: birthDate ?? DateTime.UtcNow.AddYears(-25),
            position: position,
            contacts: ValidContacts()
        );

    // ─── Constructor Tests ──────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidData_CreatesPersonnel()
    {
        var personnel = CreatePersonnel();

        personnel.FullName.Should().Be("Іван Тестенко");
        personnel.Position.Should().Be("Driver");
        personnel.Status.Should().Be("Active");
        personnel.PersonnelId.Should().Be(1);
    }


    [Fact]
    public void Constructor_Under18_ThrowsDomainException()
    {
        var birthDate = DateTime.UtcNow.AddYears(-17);

        var act = () => new Personnel(1, "Тест Тестенко", birthDate, "Driver", ValidContacts());

        act.Should().Throw<DomainException>()
            .WithMessage("*18 years old*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_EmptyPosition_ThrowsDomainException(string position)
    {
        var act = () => new Personnel(1, "Тест Тестенко", DateTime.UtcNow.AddYears(-25), position, ValidContacts());

        act.Should().Throw<DomainException>()
            .WithMessage("*Position*");
    }

    [Fact]
    public void Constructor_NullContacts_ThrowsDomainException()
    {
        var act = () => new Personnel(1, "Тест Тестенко", DateTime.UtcNow.AddYears(-25), "Driver", null!);

        act.Should().Throw<DomainException>()
            .WithMessage("*Contacts*");
    }

    // ─── UpdatePersonalInfo Tests ───────────────────────────────────────────────

    [Fact]
    public void UpdatePersonalInfo_ValidData_UpdatesFields()
    {
        var personnel = CreatePersonnel();
        var newBirthDate = DateTime.UtcNow.AddYears(-30);

        personnel.UpdatePersonalInfo("Нове Ім'я", newBirthDate, "Conductor");

        personnel.FullName.Should().Be("Нове Ім'я");
        personnel.BirthDate.Should().Be(newBirthDate);
        personnel.Position.Should().Be("Conductor");
    }

    [Fact]
    public void UpdatePersonalInfo_EmptyName_ThrowsDomainException()
    {
        var personnel = CreatePersonnel();

        var act = () => personnel.UpdatePersonalInfo("", DateTime.UtcNow.AddYears(-25), "Driver");

        act.Should().Throw<DomainException>();
    }

    // ─── UpdateStatus Tests ─────────────────────────────────────────────────────

    [Theory]
    [InlineData("Active")]
    [InlineData("Inactive")]
    [InlineData("OnLeave")]
    [InlineData("Terminated")]
    public void UpdateStatus_ValidStatus_ChangesStatus(string status)
    {
        var personnel = CreatePersonnel();

        personnel.UpdateStatus(status);

        personnel.Status.Should().Be(status);
    }

    [Fact]
    public void UpdateStatus_InvalidStatus_ThrowsInvalidPersonnelStatusException()
    {
        var personnel = CreatePersonnel();

        var act = () => personnel.UpdateStatus("SuperAdmin");

        act.Should().Throw<InvalidPersonnelStatusException>();
    }

    // ─── IsActive / GetAge Tests ────────────────────────────────────────────────

    [Fact]
    public void IsActive_WhenStatusIsActive_ReturnsTrue()
    {
        var personnel = CreatePersonnel();

        personnel.IsActive().Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenStatusIsTerminated_ReturnsFalse()
    {
        var personnel = CreatePersonnel();
        personnel.UpdateStatus("Terminated");

        personnel.IsActive().Should().BeFalse();
    }

    [Fact]
    public void GetAge_Returns_CorrectAge()
    {
        var birthDate = DateTime.UtcNow.AddYears(-30);
        var personnel = CreatePersonnel(birthDate: birthDate);

        personnel.GetAge().Should().Be(30);
    }

    // ─── Documents Tests ────────────────────────────────────────────────────────

    [Fact]
    public void AddDocument_ValidDocument_AddsToCollection()
    {
        var personnel = CreatePersonnel();
        var doc = new PersonnelDocumentInfo("DriverLicense");

        personnel.AddDocument(doc);

        personnel.Documents.Should().HaveCount(1);
        personnel.Documents.First().Type.Should().Be("DriverLicense");
    }

    [Fact]
    public void AddDocument_NullDocument_ThrowsDomainException()
    {
        var personnel = CreatePersonnel();

        var act = () => personnel.AddDocument(null!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoveDocument_ExistingType_RemovesFromCollection()
    {
        var personnel = CreatePersonnel();
        personnel.AddDocument(new PersonnelDocumentInfo("DriverLicense"));

        personnel.RemoveDocument("DriverLicense");

        personnel.Documents.Should().BeEmpty();
    }
}