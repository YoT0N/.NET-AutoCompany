using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Domain.Entities;

namespace RoutingService.Dal.Data.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trip");

            builder.HasKey(t => t.TripId);

            builder.Property(t => t.TripId)
                .HasColumnName("TripId");

            builder.Property(t => t.SheetId)
                .IsRequired()
                .HasColumnName("SheetId");

            builder.Property(t => t.ScheduledDeparture)
                .IsRequired()
                .HasColumnType("time")
                .HasColumnName("ScheduledDeparture");

            builder.Property(t => t.ActualDeparture)
                .HasColumnType("time")
                .HasColumnName("ActualDeparture");

            builder.Property(t => t.Completed)
                .HasDefaultValue(false)
                .HasColumnName("Completed");

            builder.HasOne(t => t.RouteSheet)
                .WithMany(rs => rs.Trips)
                .HasForeignKey(t => t.SheetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}