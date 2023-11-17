using MediatR;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Infrasctructure.RepositoryPattern;
using System.Data;

namespace N5.Challenge.Application.Queries
{
    public class GetEmployeePermissionsQueryHandler
    {
        public class Query : IRequest<List<EmployeePermissionDto>>
        {
            public Guid EmployeeId { get; set; }

            public Query(Guid employeeId)
            {
                EmployeeId = employeeId;
            }
        }

        public class Handler : IRequestHandler<Query, List<EmployeePermissionDto>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<List<EmployeePermissionDto>> Handle(
                Query query,
                CancellationToken cancellationToken
            )
            {
                using (var repository = _unitOfWork.GetPermissionRepository())
                {
                    return await repository.GetEmployeePermissions(query.EmployeeId);
                }
            }
        }
    }
}
