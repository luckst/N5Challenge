using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using N5.Challenge.Domain;

namespace N5.Challenge.Infrasctructure.EntityConfiguration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(e => e.Id);

            builder
               .HasIndex(u => new { u.EmployeeId, u.PermissionTypeId })
               .IsUnique();

            builder.HasOne(u => u.Employee)
                            .WithMany(c => c.Permissions)
                            .HasForeignKey(u => u.EmployeeId)
                            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.PermissionType)
                            .WithMany(s => s.Permissions)
                            .HasForeignKey(u => u.PermissionTypeId)
                            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CreatedOn)
                .IsRequired(true)
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(c => c.Enabled)
                .IsRequired(true);
        }
    }
}
