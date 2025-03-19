﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup.GetByUserDepartment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup.GetDepartment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup.GetUserDepartment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup.SyncDepartment;

namespace MakeItSimple.WebApi.Controllers.Setup.DepartmentController
{
    [Route("api/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SyncDepartment([FromBody] SyncDepartmentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                }

                var result = await _mediator.Send(command);
                if (result.IsWarning)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("page")]
        public async Task<IActionResult> GetDepartment([FromQuery] GetDepartmentQuery query)
        {
            try
            {
                var department = await _mediator.Send(query);

                Response.AddPaginationHeader(

                department.CurrentPage,
                department.PageSize,
                department.TotalCount,
                department.TotalPages,
                department.HasPreviousPage,
                department.HasNextPage

                );

                var result = new
                {
                    department,
                    department.CurrentPage,
                    department.PageSize,
                    department.TotalCount,
                    department.TotalPages,
                    department.HasPreviousPage,
                    department.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user-department")]
        public async Task<IActionResult> GetUserDepartment([FromQuery] GetUserDepartmentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("by-user-department")]
        public async Task<IActionResult> GetUserByDepartment([FromQuery] GetUserByDepartmentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }




    }
}
