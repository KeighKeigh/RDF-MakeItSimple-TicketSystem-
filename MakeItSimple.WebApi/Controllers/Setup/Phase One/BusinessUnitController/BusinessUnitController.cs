﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.BusinessUnitSetup.GetBusinessUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.BusinessUnitSetup.SyncBusinessUnit;

namespace MakeItSimple.WebApi.Controllers.Setup.BusinessUnitController
{
    [Route("api/business-unit")]
    [ApiController]
    public class BusinessUnitController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BusinessUnitController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SyncBusinessUnit([FromBody] SyncBusinessUnitCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                }
                var results = await _mediator.Send(command);
                if(results.IsWarning)
                {
                    return BadRequest(results);
                }
                return Ok(results); 
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetBusinessUnit([FromQuery] GetBussinessUnitQuery query)
        {
            try
            {
                var businessUnit = await _mediator.Send(query);

                Response.AddPaginationHeader(

                businessUnit.CurrentPage,
                businessUnit.PageSize,
                businessUnit.TotalCount,
                businessUnit.TotalPages,
                businessUnit.HasPreviousPage,
                businessUnit.HasNextPage

                );

                var result = new
                {
                    businessUnit,
                    businessUnit.CurrentPage,
                    businessUnit.PageSize,
                    businessUnit.TotalCount,
                    businessUnit.TotalPages,
                    businessUnit.HasPreviousPage,
                    businessUnit.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
