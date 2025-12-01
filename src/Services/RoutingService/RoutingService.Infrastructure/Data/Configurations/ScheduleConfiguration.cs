using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Core.Entities;

namespace RoutingService.Dal.Data.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedule");

            builder.HasKey(s => s.ScheduleId);

            builder.Property(s => s.ScheduleId)
                .HasColumnName("ScheduleId");

            builder.Property(s => s.RouteId)
                .IsRequired()
                .HasColumnName("RouteId");

            builder.Property(s => s.DepartureTime)
                .IsRequired()
                .HasColumnType("time")
                .HasColumnName("DepartureTime");

            builder.Property(s => s.ArrivalTime)
                .IsRequired()
                .HasColumnType("time")
                .HasColumnName("ArrivalTime");

            builder.HasOne(s => s.Route)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}