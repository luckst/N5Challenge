using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Entities.Models;
using N5.Challenge.Infrasctructure;
using System.Net.Http.Json;

namespace N5.Challenge.Integration.Tests
{
    public class PermissionControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ServiceDbContext _context;
        private static readonly Guid TestEmployeeId = Guid.NewGuid();
        private static readonly Guid TestPermissionTypeId = Guid.NewGuid();

        public PermissionControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ServiceDbContext>();
        }

        [Fact]
        public async Task CreatePermission_ShouldSucceed()
        {
            // Arrange
            var permission = new AddPermissionModel
            {
                EmployeeId = TestEmployeeId,
                PermissionTypeId = TestPermissionTypeId
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/permissions", permission);

            // Assert
            Assert.True(response.IsSuccessStatusCode, "Permission creation failed");
            var createdPermission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.EmployeeId == TestEmployeeId && p.PermissionTypeId == TestPermissionTypeId);
            Assert.NotNull(createdPermission);
        }

        [Fact]
        public async Task UpdatePermission_ShouldSucceed()
        {
            // Arrange
            var permission = new UpdatePermissionModel
            {
                EmployeeId = TestEmployeeId,
                PermissionTypeId = TestPermissionTypeId,
                Enabled = false
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/permissions", permission);

            // Assert
            Assert.True(response.IsSuccessStatusCode, "Permission update failed");
        }

        [Fact]
        public async Task GetPermission_ShouldSucceed()
        {
            // Act
            var response = await _client.GetAsync($"/api/permissions/{TestEmployeeId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, "Permission retrieval failed");
            var permissions = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeePermissionDto>>();
            Assert.NotNull(permissions);
            Assert.Contains(permissions, p =>
                p.EmployeeId == TestEmployeeId &&
                p.PermissionTypeId == TestPermissionTypeId);
        }

        public void Dispose()
        {
            CleanupTestData().Wait();
        }

        private async Task CleanupTestData()
        {
            try
            {
                var permissionsToDelete = await _context.Permissions
                    .Where(p => p.EmployeeId == TestEmployeeId)
                    .ToListAsync();

                _context.Permissions.RemoveRange(permissionsToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }
    }
}