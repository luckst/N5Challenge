using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using N5.Challenge.Domain;

namespace N5.Challenge.Infrasctructure.EntityConfiguration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(c => c.DocumentNumber)
                .IsRequired(true)
                .HasMaxLength(20);

            builder.Property(c => c.FullName)
                .IsRequired(true)
                .HasMaxLength(150);

            builder.Property(c => c.Email)
                .IsRequired(true)
                .HasMaxLength(250);

            builder.Property(c => c.Phone)
                .IsRequired(true)
                .HasMaxLength(20);

            builder.Property(c => c.CreatedOn)
                .IsRequired(true)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
