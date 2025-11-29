namespace TechnicalService.Domain.Constants;

public static class BusStatusConstants
{
    public const string Active = "Активний";
    public const string InMaintenance = "На обслуговуванні";
    public const string OutOfService = "Виведений з експлуатації";
    public const string UnderRepair = "На ремонті";

    public static readonly string[] AllStatuses =
    {
        Active,
        InMaintenance,
        OutOfService,
        UnderRepair
    };

    public static bool IsValid(string status)
    {
        return AllStatuses.Contains(status);
    }
}