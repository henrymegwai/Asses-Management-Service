using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CloudWorks.Data.Entities;

namespace CloudWorks.Data.EntityConfigurations;
public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Name).HasColumnName("name");

        builder.Property(x => x.SiteId).HasColumnName("site_id");

        builder.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId);

        builder.HasMany(x => x.SiteProfiles).WithMany().UsingEntity(x => x.ToTable("booking_site_profiles"));

        builder.HasMany(x => x.AccessPoints).WithMany().UsingEntity(x => x.ToTable("booking_access_points"));

        builder.HasMany(x => x.Schedules).WithOne(x => x.Booking).HasForeignKey(x => x.BookingId);
    }
}
