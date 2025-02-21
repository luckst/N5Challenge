using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Entities.Models;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.Exceptions;
using N5.Challenge.Infrasctructure.KafkaConfig;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.Application.Commands
{
    public class AddPermissionCommandHandler
    {
        public class Command : AddPermissionModel, IRequest<Unit>
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

                    if (employeePermission != null)
                    {
                        throw new DuplicatedPermissionException();
                    }

                    var permission = new Permission
                    {
                        PermissionTypeId = command.PermissionTypeId,
                        CreatedOn = DateTime.UtcNow,
                        EmployeeId = command.EmployeeId,
                        Enabled = true
                    };

                    await repository.CreateAsync(permission);
                    await _unitOfWork.CommitAsync();
                    try
                    {
                        await _elasticSearchRepository.PersistAsync(permission);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        try
                        {
                            await _operationProducer.SendOperationAsync("create");
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