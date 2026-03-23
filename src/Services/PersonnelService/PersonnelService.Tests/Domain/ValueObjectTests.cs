using FluentAssertions;
using PersonnelService.Domain.Exceptions;
using PersonnelService.Domain.ValueObjects;
using Xunit;

namespace PersonnelService.Tests.Domain;

public class ValueObjectTests
{
    // ─── PersonnelContactsVO ────────────────────────────────────────────────────

    [Fact]
    public void PersonnelContactsVO_ValidData_CreatesSuccessfully()
    {
        var contacts = new PersonnelContactsVO("+380501234567", "test@example.com", "Київ");

        contacts.Phone.Should().Be("+380501234567");
        contacts.Email.Should().Be("test@example.com");
        contacts.Address.Should().Be("Київ");
    }

    [Fact]
    public void PersonnelContactsVO_InvalidEmail_ThrowsDomainException()
    {
        var act = () => new PersonnelContactsVO("+380501234567", "not-an-email", "Київ");

        act.Should().Throw<DomainException>()
            .WithMessage("*Invalid email*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void PersonnelContactsVO_EmptyPhone_ThrowsDomainException(string phone)
    {
        var act = () => new PersonnelContactsVO(phone, "test@example.com", "Київ");

        act.Should().Throw<DomainException>()
            .WithMessage("*Phone*");
    }

    [Fact]
    public void PersonnelContactsVO_EqualityByValue_WorksCorrectly()
    {
        var contacts1 = new PersonnelContactsVO("+380501234567", "test@example.com", "Київ");
        var contacts2 = new PersonnelContactsVO("+380501234567", "test@example.com", "Київ");

        contacts1.Should().Be(contacts2);
    }

    [Fact]
    public void PersonnelContactsVO_DifferentPhone_NotEqual()
    {
        var contacts1 = new PersonnelContactsVO("+380501234567", "test@example.com", "Київ");
        var contacts2 = new PersonnelContactsVO("+380509999999", "test@example.com", "Київ");

        contacts1.Should().NotBe(contacts2);
    }

    // ─── ExaminationMetricsVO ───────────────────────────────────────────────────

    [Fact]
    public void ExaminationMetricsVO_ValidData_CreatesSuccessfully()
    {
        var metrics = new ExaminationMetricsVO(178, 75, "120/80", "1.0/1.0");

        metrics.Height.Should().Be(178);
        metrics.Weight.Should().Be(75);
        metrics.BloodPressure.Should().Be("120/80");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(300)]
    public void ExaminationMetricsVO_InvalidHeight_ThrowsDomainException(int height)
    {
        var act = () => new ExaminationMetricsVO(height, 75, "120/80", "1.0/1.0");

        act.Should().Throw<DomainException>()
            .WithMessage("*height*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(400)]
    public void ExaminationMetricsVO_InvalidWeight_ThrowsDomainException(int weight)
    {
        var act = () => new ExaminationMetricsVO(178, weight, "120/80", "1.0/1.0");

        act.Should().Throw<DomainException>()
            .WithMessage("*weight*");
    }

    [Fact]
    public void ExaminationMetricsVO_CalculateBMI_ReturnsCorrectValue()
    {
        var metrics = new ExaminationMetricsVO(180, 81, "120/80", "1.0/1.0");

        var bmi = metrics.CalculateBMI();

        bmi.Should().BeApproximately(25.0, 0.1);
    }

    [Fact]
    public void ExaminationMetricsVO_GetBMICategory_Normal()
    {
        var metrics = new ExaminationMetricsVO(180, 75, "120/80", "1.0/1.0");

        metrics.GetBMICategory().Should().Be("Normal");
    }


    // ─── BusInfoVO ──────────────────────────────────────────────────────────────

    [Fact]
    public void BusInfoVO_ValidData_CreatesSuccessfully()
    {
        var busInfo = new BusInfoVO("AA1234BB", "Mercedes-Benz");

        busInfo.BusCountryNumber.Should().Be("AA1234BB");
        busInfo.Brand.Should().Be("Mercedes-Benz");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void BusInfoVO_EmptyBusNumber_ThrowsDomainException(string busNumber)
    {
        var act = () => new BusInfoVO(busNumber, "Mercedes-Benz");

        act.Should().Throw<DomainException>()
            .WithMessage("*Bus country number*");
    }

    [Fact]
    public void BusInfoVO_EqualityByValue_WorksCorrectly()
    {
        var bus1 = new BusInfoVO("AA1234BB", "Mercedes-Benz");
        var bus2 = new BusInfoVO("AA1234BB", "Mercedes-Benz");

        bus1.Should().Be(bus2);
    }

    // ─── RouteInfoVO ────────────────────────────────────────────────────────────-
    // Працює)

    [Fact]
    public void RouteInfoVO_ValidData_CreatesSuccessfully()
    {
        var route = new RouteInfoVO("15А", 28.5);

        route.RouteNumber.Should().Be("15А");
        route.DistanceKm.Should().Be(28.5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void RouteInfoVO_NonPositiveDistance_ThrowsDomainException(double distance)
    {
        var act = () => new RouteInfoVO("15А", distance);

        act.Should().Throw<DomainException>()
            .WithMessage("*Distance*");
    }
}