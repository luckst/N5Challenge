using MediatR;
using Microsoft.AspNetCore.Mvc;
using N5.Challenge.Application.Queries;
using N5.Challenge.Entities.Dtos;

namespace N5.Challenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{employeeId}")]
        public async Task<List<EmployeePermissionDto>> GetEmployeePermissions(Guid employeeId)
        {
            return await _mediator.Send(new GetEmployeePermissionsQueryHandler.Query(employeeId));
        }
    }
}
