using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CloudWorks.Data.Entities;


namespace CloudWorks.Data.EntityConfigurations;
public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Email).HasColumnName("email");

        builder.Property(x => x.IdentityId).HasColumnName("identity_id").IsRequired(false);

        builder.HasIndex(x => x.Email).IsUnique(true);
    }
}
