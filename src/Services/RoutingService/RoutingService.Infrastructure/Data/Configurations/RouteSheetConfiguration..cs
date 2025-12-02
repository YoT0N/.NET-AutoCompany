using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Domain.Entities;

namespace RoutingService.Dal.Data.Configurations
{
    public class RouteSheetConfiguration : IEntityTypeConfiguration<RouteSheet>
    {
        public void Configure(EntityTypeBuilder<RouteSheet> builder)
        {
            builder.ToTable("RouteSheet");

            builder.HasKey(rs => rs.SheetId);

            builder.Property(rs => rs.SheetId)
                .HasColumnName("SheetId");

            builder.Property(rs => rs.RouteId)
                .IsRequired()
                .HasColumnName("RouteId");

            builder.Property(rs => rs.BusId)
                .IsRequired()
                .HasColumnName("BusId");

            builder.Property(rs => rs.SheetDate)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("SheetDate");

            builder.HasIndex(rs => new { rs.RouteId, rs.BusId, rs.SheetDate })
                .IsUnique();

            builder.HasOne(rs => rs.Route)
                .WithMany(r => r.RouteSheets)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.BusInfo)
                .WithMany(b => b.RouteSheets)
                .HasForeignKey(rs => rs.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(rs => rs.Trips)
                .WithOne(t => t.RouteSheet)
                .HasForeignKey(t => t.SheetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}