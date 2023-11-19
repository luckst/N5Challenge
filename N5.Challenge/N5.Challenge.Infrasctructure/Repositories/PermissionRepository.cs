using Microsoft.EntityFrameworkCore;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;

namespace N5.Challenge.Infrasctructure.Repositories
{
    public class PermissionRepository : IPermissionRepository 
    {
        private readonly ServiceDbContext _context;

        public PermissionRepository(ServiceDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Permission entity)
        {
            await _context.Permissions.AddAsync(entity);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<Permission> GetAsync(Guid id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task<Permission> GetEmployeePermission(Guid employeeId, Guid permissionTypeId)
        {
            return await _context.Permissions.Include(p => p.Employee).Include(p => p.PermissionType).Where(p => p.EmployeeId == employeeId && p.PermissionTypeId == permissionTypeId).FirstOrDefaultAsync();
        }

        public async Task<List<EmployeePermissionDto>> GetEmployeePermissions(Guid employeeId)
        {
            return await _context.Permissions.Include(p => p.Employee).Include(p => p.PermissionType).Where(p => p.EmployeeId == employeeId).Select(p => new EmployeePermissionDto
            {

            }).ToListAsync();
        }

        public Task UpdateAsync(Permission entity)
        {
            _context.Update(entity);
            return Task.CompletedTask;
        }
    }
}
