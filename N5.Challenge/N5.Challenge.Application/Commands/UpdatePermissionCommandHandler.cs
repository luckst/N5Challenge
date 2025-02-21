using MediatR;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Models;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.Exceptions;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using N5.Challenge.Infrasctructure.RepositoryPattern;
using System.Diagnostics;
using System.Security;

namespace N5.Challenge.Application.Commands
{
    public class UpdatePermissionCommandHandler
    {
        public class Command : UpdatePermissionModel, IRequest<Unit>
        {
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IElasticSearchRepository<Permission> _elasticSearchRepository;
            private readonly IOperationProducer _operationProducer;

            public Handler(IUnitOfWork unitOfWork, IElasticSearchRepository<Permission> elasticSearchRepository, IOperationProducer operationProducer)
            {
                _unitOfWork = unitOfWork;
                _elasticSearchRepository = elasticSearchRepository;
                _operationProducer = operationProducer;
            }

            public async Task<Unit> Handle(
                Command command,
                CancellationToken cancellationToken
            )
            {
                if (command.EmployeeId == Guid.Empty)
                {
                    throw new ArgumentNullException("Employee id is required");
                }

                if (command.PermissionTypeId == Guid.Empty)
                {
                    throw new ArgumentNullException("Permission type id is required");
                }

                using (var repository = _unitOfWork.GetPermissionRepository())
                {
                    var employeePermission = await repository.GetEmployeePermission(command.EmployeeId, command.PermissionTypeId);

                    if (employeePermission == null)
                    {
                        throw new PermissionNotFoundException();
                    }

                    employeePermission.Enabled = command.Enabled;
                    await repository.UpdateAsync(employeePermission);
                    await _unitOfWork.CommitAsync();
                    try
                    {
                        await _elasticSearchRepository.PersistAsync(employeePermission);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        try
                        {
                            await _operationProducer.SendOperationAsync("modify");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                return Unit.Value;
            }
        }
    }
}