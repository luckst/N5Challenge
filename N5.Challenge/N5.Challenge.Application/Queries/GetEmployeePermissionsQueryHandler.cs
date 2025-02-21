using MediatR;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using N5.Challenge.Infrasctructure.RepositoryPattern;
using System.Data;
using System.Diagnostics;

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
            private readonly IOperationProducer _operationProducer;

            public Handler(IUnitOfWork unitOfWork, IOperationProducer operationProducer)
            {
                _unitOfWork = unitOfWork;
                _operationProducer = operationProducer;
            }

            public async Task<List<EmployeePermissionDto>> Handle(
                Query query,
                CancellationToken cancellationToken
            )
            {
                try
                {
                    using (var repository = _unitOfWork.GetPermissionRepository())
                    {
                        var permissions = await repository.GetEmployeePermissions(query.EmployeeId);

                        try
                        {
                            await _operationProducer.SendOperationAsync("request");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }

                        return permissions;
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    throw new ApplicationException("An error occurred while retrieving employee permissions.", ex);
                }
            }
        }
    }
}
