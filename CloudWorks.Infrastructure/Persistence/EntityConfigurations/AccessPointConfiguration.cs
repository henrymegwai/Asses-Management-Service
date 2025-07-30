using CloudWorks.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudWorks.Infrastructure.Persistence.EntityConfigurations;
public sealed class AccessPointConfiguration : IEntityTypeConfiguration<AccessPoint>
{
    public void Configure(EntityTypeBuilder<AccessPoint> builder)
    {
        builder.ToTable("access_points");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Name).HasColumnName("name");

        builder.Property(x => x.SiteId).HasColumnName("site_id");

        builder.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId);
    }
}
