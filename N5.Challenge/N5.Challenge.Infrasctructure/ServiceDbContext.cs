using Microsoft.EntityFrameworkCore;
using N5.Challenge.Domain;

namespace N5.Challenge.Infrasctructure
{
    public class ServiceDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public ServiceDbContext(DbContextOptions<ServiceDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
