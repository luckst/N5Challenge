using MediatR;
using Microsoft.AspNetCore.Mvc;
using N5.Challenge.Application.Commands;
using N5.Challenge.Application.Queries;
using N5.Challenge.Entities.Dtos;
using Serilog.Core;

namespace N5.Challenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Logger _logger;

        public PermissionsController(IMediator mediator, Logger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{employeeId}")]
        public async Task<List<EmployeePermissionDto>> Get(Guid employeeId)
        {
            _logger.Information($"Calling get permissions - employee id: {employeeId}");
            return await _mediator.Send(new GetEmployeePermissionsQueryHandler.Query(employeeId));
        }

        [HttpPost()]
        public async Task Add(AddPermissionCommandHandler.Command command)
        {
            _logger.Information($"Calling add permissions - employee id: {command.EmployeeId}");
            await _mediator.Send(command);
        }

        [HttpPut()]
        public async Task Update(UpdatePermissionCommandHandler.Command command)
        {
            _logger.Information($"Calling update permissions - employee id: {command.EmployeeId} - enabled: {command.Enabled}");
            await _mediator.Send(command);
        }
    }
}
