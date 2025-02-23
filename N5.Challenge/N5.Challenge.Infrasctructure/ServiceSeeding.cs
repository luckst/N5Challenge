using Microsoft.EntityFrameworkCore;
using N5.Challenge.Domain;

namespace N5.Challenge.Infrasctructure
{
    public class ServiceSeeding
    {
        public async Task SeedAsync(ServiceDbContext context)
        {
            try
            {
                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            if (!context.Employees.Any())
            {
                var employess = new List<Employee>() {
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        DocumentNumber = "1234567890",
                        Email = "test1@email.com",
                        Phone = "3214567899",
                        FullName = "Test 1 Emp"
                    },
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        DocumentNumber = "1234567891",
                        Email = "test2@email.com",
                        Phone = "3214567891",
                        FullName = "Test 2 Emp"
                    },
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        DocumentNumber = "1234567892",
                        Email = "test3@email.com",
                        Phone = "3214567892",
                        FullName = "Test 3 Emp"
                    },
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        DocumentNumber = "1234567893",
                        Email = "test4@email.com",
                        Phone = "3214567893",
                        FullName = "Test 4 Emp"
                    }
                };
                context.Employees.AddRange(employess);
                await context.SaveChangesAsync();
            }

            if (!context.PermissionTypes.Any())
            {
                var permissionTypes = new List<PermissionType>() {
                    new PermissionType
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        Name = "Create"
                    },
                    new PermissionType
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        Name = "Update"
                    },
                    new PermissionType
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        Name = "Delete"
                    }
                };
                context.PermissionTypes.AddRange(permissionTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
