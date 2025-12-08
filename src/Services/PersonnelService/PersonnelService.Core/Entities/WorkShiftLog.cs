using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Domain.Entities
{
    public class WorkShiftLog : BaseEntity
    {
        [BsonElement("personnelId")]
        public int PersonnelId { get; private set; }

        [BsonElement("shiftDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ShiftDate { get; private set; }

        [BsonElement("startTime")]
        public string StartTime { get; private set; } = string.Empty;

        [BsonElement("endTime")]
        public string EndTime { get; private set; } = string.Empty;

        [BsonElement("bus")]
        public BusInfoVO Bus { get; private set; }

        [BsonElement("route")]
        public RouteInfoVO Route { get; private set; }

        [BsonElement("status")]
        public string Status { get; private set; } = "Scheduled";

        public WorkShiftLog(
            int personnelId,
            DateTime shiftDate,
            string startTime,
            string endTime,
            BusInfoVO bus,
            RouteInfoVO route)
        {
            if (personnelId <= 0)
                throw new DomainException("PersonnelId must be positive");

            if (string.IsNullOrWhiteSpace(startTime))
                throw new DomainException("Start time cannot be empty");

            if (string.IsNullOrWhiteSpace(endTime))
                throw new DomainException("End time cannot be empty");

            PersonnelId = personnelId;
            ShiftDate = shiftDate;
            StartTime = startTime;
            EndTime = endTime;
            Bus = bus ?? throw new DomainException("Bus info cannot be null");
            Route = route ?? throw new DomainException("Route info cannot be null");
            Status = "Scheduled";
        }

        [BsonConstructor]
        private WorkShiftLog() { }

        public void UpdateShiftDetails(
            DateTime shiftDate,
            string startTime,
            string endTime,
            BusInfoVO bus,
            RouteInfoVO route)
        {
            if (string.IsNullOrWhiteSpace(startTime))
                throw new DomainException("Start time cannot be empty");

            if (string.IsNullOrWhiteSpace(endTime))
                throw new DomainException("End time cannot be empty");

            ShiftDate = shiftDate;
            StartTime = startTime;
            EndTime = endTime;
            Bus = bus ?? throw new DomainException("Bus info cannot be null");
            Route = route ?? throw new DomainException("Route info cannot be null");
            Touch();
        }

        public void UpdateStatus(string status)
        {
            var validStatuses = new[] { "Scheduled", "InProgress", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
                throw new DomainException($"Invalid shift status: {status}");

            Status = status;
            Touch();
        }

        public void Complete()
        {
            if (Status == "Cancelled")
                throw new DomainException("Cannot complete a cancelled shift");

            Status = "Completed";
            Touch();
        }

        public void Cancel()
        {
            if (Status == "Completed")
                throw new DomainException("Cannot cancel a completed shift");

            Status = "Cancelled";
            Touch();
        }

        public bool IsCompleted() => Status == "Completed";
        public bool IsCancelled() => Status == "Cancelled";
    }
}