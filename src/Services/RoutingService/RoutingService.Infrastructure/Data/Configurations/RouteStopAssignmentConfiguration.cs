using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Core.Entities;

namespace RoutingService.Dal.Data.Configurations
{
    public class RouteStopAssignmentConfiguration : IEntityTypeConfiguration<RouteStopAssignment>
    {
        public void Configure(EntityTypeBuilder<RouteStopAssignment> builder)
        {
            builder.ToTable("Route_Stop");

            builder.HasKey(rsa => new { rsa.RouteId, rsa.StopId });

            builder.Property(rsa => rsa.RouteId)
                .HasColumnName("RouteId");

            builder.Property(rsa => rsa.StopId)
                .HasColumnName("StopId");

            builder.Property(rsa => rsa.StopOrder)
                .IsRequired()
                .HasColumnName("StopOrder");

            builder.HasIndex(rsa => new { rsa.RouteId, rsa.StopOrder })
                .HasDatabaseName("IX_RouteStop_Order");

            builder.HasOne(rsa => rsa.Route)
                .WithMany(r => r.RouteStopAssignments)
                .HasForeignKey(rsa => rsa.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rsa => rsa.RouteStop)
                .WithMany(rs => rs.RouteStopAssignments)
                .HasForeignKey(rsa => rsa.StopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}