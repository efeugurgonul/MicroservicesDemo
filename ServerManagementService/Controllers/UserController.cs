using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Features.Users.Commands.Create;
using ServerManagementService.Features.Users.Commands.Delete;
using ServerManagementService.Features.Users.Commands.Update;
using ServerManagementService.Features.Users.Queries.Detail;
using ServerManagementService.Features.Users.Queries.List;

namespace ServerManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT doğrulaması gerektirir
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll([FromQuery] bool? isActive, [FromQuery] int? organizationId)
        {
            var query = new GetAllUsersQuery
            {
                Filter = new UserFilter
                {
                    IsActive = isActive,
                    OrganizationId = organizationId
                }
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetById(int userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { userId = result }, result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> Update(int userId, [FromBody] UpdateUserCommand command)
        {
            command.UserId = userId;
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(int userId)
        {
            var command = new DeleteUserCommand { UserId = userId };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}