﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServerManagentService.Domain.Entities;
using ServerManagentService.Features.Organizations.Commands.Create;
using ServerManagentService.Features.Organizations.Commands.Delete;
using ServerManagentService.Features.Organizations.Commands.Update;
using ServerManagentService.Features.Organizations.Queries.Detail;
using ServerManagentService.Features.Organizations.Queries.List;

namespace ServerManagentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Organization>>> GetAll([FromQuery] int? activeStatus)
        {            
            var query = new GetAllOrganizationsQuery
            {
                Filter = new OrganizationFilter
                {
                    ActiveStatus = activeStatus
                }
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{organizationId}")]
        public async Task<ActionResult<Organization>> GetById(int organizationId)
        {
            var query = new GetOrganizationByIdQuery { OrganizationId = organizationId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(Organization organization)
        {
            var command = new CreateOrganizationCommand { Organization = organization };
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { code = result }, result);
        }

        [HttpPut("{organizationId}")]
        public async Task<ActionResult> Update(int organizationId, Organization organization)
        {
            var command = new UpdateOrganizationCommand { OrganizationId = organizationId, Organization = organization };
            var result = await _mediator.Send(command);
            
            if(result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{organizationId}")]
        public async Task<ActionResult> Delete(int organizationId)
        {
            var command = new DeleteOrganizationCommand { OrganizationId = organizationId };
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound();
            
            return NoContent();
        }
    }
}
