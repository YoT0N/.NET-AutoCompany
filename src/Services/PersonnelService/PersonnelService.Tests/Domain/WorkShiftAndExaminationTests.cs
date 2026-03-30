using FluentAssertions;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Exceptions;
using PersonnelService.Domain.ValueObjects;
using Xunit;

namespace PersonnelService.Tests.Domain;

public class WorkShiftLogTests
{
    private static BusInfoVO ValidBus() => new("AA1234BB", "Mercedes-Benz");
    private static RouteInfoVO ValidRoute() => new("15À", 28.5);

    private static WorkShiftLog CreateShift(string status = "Scheduled") =>
        new(
            personnelId: 1,
            shiftDate: DateTime.UtcNow.Date,
            startTime: "06:00",
            endTime: "14:00",
            bus: ValidBus(),
            route: ValidRoute()
        );

    [Fact]
    public void Constructor_ValidData_SetsDefaultScheduledStatus()
    {
        var shift = CreateShift();

        shift.Status.Should().Be("Scheduled");
        shift.PersonnelId.Should().Be(1);
    }

    [Fact]
    public void Constructor_NonPositivePersonnelId_ThrowsDomainException()
    {
        var act = () => new WorkShiftLog(0, DateTime.UtcNow, "06:00", "14:00", ValidBus(), ValidRoute());

        act.Should().Throw<DomainException>()
            .WithMessage("*PersonnelId*");
    }

    [Theory]
    [InlineData("Scheduled", "Completed")]
    [InlineData("Scheduled", "Cancelled")]
    [InlineData("InProgress", "Completed")]
    public void UpdateStatus_ValidTransitions_Succeeds(string from, string to)
    {
        var shift = CreateShift();
        if (from != "Scheduled")
            shift.UpdateStatus(from);

        shift.UpdateStatus(to);

        shift.Status.Should().Be(to);
    }

    [Fact]
    public void UpdateStatus_InvalidStatus_ThrowsDomainException()
    {
        var shift = CreateShift();

        var act = () => shift.UpdateStatus("UnknownStatus");

        act.Should().Throw<DomainException>()
            .WithMessage("*Invalid shift status*");
    }

    [Fact]
    public void Complete_CancelledShift_ThrowsDomainException()
    {
        var shift = CreateShift();
        shift.Cancel();

        var act = () => shift.Complete();

        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot complete a cancelled shift*");
    }

    [Fact]
    public void Cancel_CompletedShift_ThrowsDomainException()
    {
        var shift = CreateShift();
        shift.Complete();

        var act = () => shift.Cancel();

        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot cancel a completed shift*");
    }

    [Fact]
    public void IsCompleted_AfterComplete_ReturnsTrue()
    {
        var shift = CreateShift();
        shift.Complete();

        shift.IsCompleted().Should().BeTrue();
        shift.IsCancelled().Should().BeFalse();
    }
}

public class PhysicalExaminationTests
{
    private static ExaminationMetricsVO ValidMetrics() =>
        new(178, 75, "120/80", "1.0/1.0");

    [Fact]
    public void Constructor_ValidData_CreatesExamination()
    {
        var exam = new PhysicalExamination(1, DateTime.UtcNow.AddDays(-1), "Passed", "Dr. Test", ValidMetrics());

        exam.Result.Should().Be("Passed");
        exam.DoctorName.Should().Be("Dr. Test");
        exam.PersonnelId.Should().Be(1);
    }

    [Fact]
    public void Constructor_NonPositivePersonnelId_ThrowsDomainException()
    {
        var act = () => new PhysicalExamination(0, DateTime.UtcNow, "Passed", "Dr. Test", ValidMetrics());

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_EmptyResult_ThrowsDomainException(string result)
    {
        var act = () => new PhysicalExamination(1, DateTime.UtcNow, result, "Dr. Test", ValidMetrics());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void IsPassed_WithPassedResult_ReturnsTrue()
    {
        var exam = new PhysicalExamination(1, DateTime.UtcNow.AddDays(-1), "Passed", "Dr. Test", ValidMetrics());

        exam.IsPassed().Should().BeTrue();
    }

    [Fact]
    public void IsPassed_WithFailedResult_ReturnsFalse()
    {
        var exam = new PhysicalExamination(1, DateTime.UtcNow.AddDays(-1), "Failed", "Dr. Test", ValidMetrics());

        exam.IsPassed().Should().BeFalse();
    }

    [Fact]
    public void IsRecent_ExamDateWithinThreshold_ReturnsTrue()
    {
        var exam = new PhysicalExamination(1, DateTime.UtcNow.AddDays(-100), "Passed", "Dr. Test", ValidMetrics());

        exam.IsRecent(365).Should().BeTrue();
    }

    [Fact]
    public void IsRecent_ExamDateBeyondThreshold_ReturnsFalse()
    {
        var exam = new PhysicalExamination(1, DateTime.UtcNow.AddDays(-400), "Passed", "Dr. Test", ValidMetrics());

        exam.IsRecent(365).Should().BeFalse();
    }
}