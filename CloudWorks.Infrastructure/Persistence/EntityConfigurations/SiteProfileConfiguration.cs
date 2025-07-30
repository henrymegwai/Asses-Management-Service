using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CloudWorks.Data.Entities;


namespace CloudWorks.Data.EntityConfigurations;
public sealed class SiteProfileConfiguration : IEntityTypeConfiguration<SiteProfile>
{
    public void Configure(EntityTypeBuilder<SiteProfile> builder)
    {
        builder.ToTable("site_profiles");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.SiteId).HasColumnName("site_id");

        builder.HasOne(x => x.Site).WithMany(x => x.Profiles).HasForeignKey(x => x.SiteId);

        builder.Property(x => x.ProfileId).HasColumnName("profile_id");

        builder.HasOne(x => x.Profile).WithMany().HasForeignKey(x => x.ProfileId);
    }
}
