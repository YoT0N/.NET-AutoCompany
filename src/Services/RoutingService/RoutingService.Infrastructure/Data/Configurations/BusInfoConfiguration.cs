using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutingService.Core.Entities;

namespace RoutingService.Dal.Data.Configurations
{
    public class BusInfoConfiguration : IEntityTypeConfiguration<BusInfo>
    {
        public void Configure(EntityTypeBuilder<BusInfo> builder)
        {
            builder.ToTable("BusInfo");

            builder.HasKey(b => b.BusId);

            builder.Property(b => b.BusId)
                .HasColumnName("BusId");

            builder.Property(b => b.CountryNumber)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("CountryNumber");

            builder.HasIndex(b => b.CountryNumber)
                .IsUnique();

            builder.Property(b => b.Brand)
                .HasMaxLength(50)
                .HasColumnName("Brand");

            builder.Property(b => b.Capacity)
                .HasColumnName("Capacity");

            builder.Property(b => b.YearOfManufacture)
                .HasColumnName("YearOfManufacture");

            builder.HasMany(b => b.RouteSheets)
                .WithOne(rs => rs.BusInfo)
                .HasForeignKey(rs => rs.BusId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}