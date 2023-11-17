using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using N5.Challenge.Domain;

namespace N5.Challenge.Infrasctructure.EntityConfiguration
{
    public class PermissionTypeConfiguration : IEntityTypeConfiguration<PermissionType>
    {
        public void Configure(EntityTypeBuilder<PermissionType> builder)
        {
            builder.ToTable("PermissionTypes");

            builder.HasKey(e => e.Id);

            builder.Property(c => c.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(c => c.CreatedOn)
                .IsRequired(true)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
