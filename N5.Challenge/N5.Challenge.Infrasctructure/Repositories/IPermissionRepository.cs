using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.Infrasctructure.Repositories
{
    public interface IPermissionRepository: IRepository<Permission>
    {
        Task<List<EmployeePermissionDto>> GetEmployeePermissions(Guid employeeId);
    }
}
