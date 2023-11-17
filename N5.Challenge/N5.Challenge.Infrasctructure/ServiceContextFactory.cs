using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace N5.Challenge.Infrasctructure
{
    public class ServiceContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
    {
        public ServiceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();

            optionsBuilder.UseSqlServer($"Server={Environment.GetEnvironmentVariable("DB_Server")},1433;Database=N5Challenge;User Id=sa;password=Test123!;TrustServerCertificate=true");

            return new ServiceDbContext(optionsBuilder.Options);
        }
    }
}
