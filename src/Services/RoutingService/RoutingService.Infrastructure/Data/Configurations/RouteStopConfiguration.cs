using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Core.Entities;

namespace RoutingService.Infrastructure.Data.Configurations
{
    public class RouteStopConfiguration : IEntityTypeConfiguration<RouteStop>
    {
        public void Configure(EntityTypeBuilder<RouteStop> builder)
        {
            builder.ToTable("RouteStop");

            builder.HasKey(rs => rs.StopId);

            builder.Property(rs => rs.StopId)
                .HasColumnName("StopId");

            builder.Property(rs => rs.StopName)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("StopName");

            builder.Property(rs => rs.Latitude)
                .HasColumnType("decimal(9,6)")
                .HasColumnName("Latitude");

            builder.Property(rs => rs.Longitude)
                .HasColumnType("decimal(9,6)")
                .HasColumnName("Longitude");

            builder.HasMany(rs => rs.RouteStopAssignments)
                .WithOne(rsa => rsa.RouteStop)
                .HasForeignKey(rsa => rsa.StopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}