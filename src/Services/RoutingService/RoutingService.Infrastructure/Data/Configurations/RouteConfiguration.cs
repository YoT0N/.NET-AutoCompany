using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Domain.Entities;

namespace RoutingService.Dal.Data.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.ToTable("Route");

            builder.HasKey(r => r.RouteId);

            builder.Property(r => r.RouteId)
                .HasColumnName("RouteId");

            builder.Property(r => r.RouteNumber)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("RouteNumber");

            builder.HasIndex(r => r.RouteNumber)
                .IsUnique();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("Name");

            builder.Property(r => r.DistanceKm)
                .HasColumnType("decimal(6,2)")
                .HasColumnName("DistanceKm");

            builder.Property(r => r.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(r => r.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();

            builder.HasMany(r => r.RouteStopAssignments)
                .WithOne(rsa => rsa.Route)
                .HasForeignKey(rsa => rsa.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Schedules)
                .WithOne(s => s.Route)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RouteSheets)
                .WithOne(rs => rs.Route)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}