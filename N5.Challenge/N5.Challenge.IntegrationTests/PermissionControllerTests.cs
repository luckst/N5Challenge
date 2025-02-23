using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Entities.Models;
using N5.Challenge.Infrasctructure;
using Nest;
using Xunit;

namespace N5.Challenge.IntegrationTests;

[TestCaseOrderer("N5.Challenge.IntegrationTests.PriorityOrderer", "N5.Challenge.IntegrationTests")]
public class PermissionControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ServiceDbContext _context;
    private readonly List<Guid> _createdPermissionIds = new();
    private static Guid TestEmployeeId;
    private static Guid TestPermissionTypeId;

    public PermissionControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ServiceDbContext>();
        InitializeTestData().Wait();
    }

    private async Task InitializeTestData()
    {
        // Obtener un empleado y un tipo de permiso de la base de datos
        var employee = await _context.Employees.FirstOrDefaultAsync();
        var permissionType = await _context.PermissionTypes.FirstOrDefaultAsync();

        if (employee == null || permissionType == null)
        {
            throw new InvalidOperationException("No se encontraron datos iniciales en la base de datos.");
        }

        TestEmployeeId = employee.Id;
        TestPermissionTypeId = permissionType.Id;
    }


    [Fact]
    [TestPriority(1)]
    public async Task Step1_CreatePermission_ShouldSucceed()
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
        Assert.True(response.IsSuccessStatusCode, "La creación del permiso falló");
        // Verifica que el permiso se haya guardado en la base de datos
        var createdPermission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.EmployeeId == TestEmployeeId && p.PermissionTypeId == TestPermissionTypeId);
        Assert.NotNull(createdPermission);
    }

    [Fact]
    [TestPriority(2)]
    public async Task Step2_UpdatePermission_ShouldSucceed()
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
        Assert.True(response.IsSuccessStatusCode, "La actualización del permiso falló");
    }

    [Fact]
    [TestPriority(3)]
    public async Task Step3_GetPermission_ShouldSucceed()
    {
        // Act
        var response = await _client.GetAsync($"/api/permissions/{TestEmployeeId}");

        // Assert
        Assert.True(response.IsSuccessStatusCode, "La obtención del permiso falló");
        var permissions = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeePermissionDto>>();
        Assert.NotNull(permissions);
        Assert.Contains(permissions, p =>
            p.EmployeeId == TestEmployeeId &&
            p.PermissionTypeId == TestPermissionTypeId);
    }

    private async Task CleanupTestData()
    {
        try
        {
            // Limpiar permisos creados
            var permissionsToDelete = await _context.Permissions
                .Where(p => p.EmployeeId == TestEmployeeId)
                .ToListAsync();

            _context.Permissions.RemoveRange(permissionsToDelete);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Limpieza completada: {permissionsToDelete.Count} permisos eliminados");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error durante la limpieza: {ex.Message}");
        }
    }

    ~PermissionControllerTests()
    {
        CleanupTestData().Wait();
    }
}