﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.GetUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.GetUserByPermission;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.UpdateProfilePic;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.UpdateUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.UpdateUserStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.UserChangePassword;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.UserResetPassword;
using static NuGet.Packaging.PackagingConstants;


namespace MakeItSimple.WebApi.Controllers.UserManagement.UserAccount
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;


        public UserController(IMediator mediator, ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }



        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] GetUsersQuery query)
        {
            try
            {
                //var cacheKey = $"users-{query.PageNumber}-{query.PageSize}";
                //var cachedUsers = await _cacheService.GetCacheAsync(cacheKey);

                //if (cachedUsers != null)
                //{
                //    return Ok(Result.Success(cachedUsers));
                //}

                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasPreviousPage,
                    users.HasNextPage
                );

                var result = new
                {
                    users,
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasPreviousPage,
                    users.HasNextPage
                };

                //await _cacheService.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        //[HttpGet("GetUser")]
        //public async Task<IActionResult> GetUser([FromQuery] GetUsersQuery query)
        //{
        //    try
        //    {

        //        var users = await _mediator.Send(query);

        //        Response.AddPaginationHeader(
        //            users.CurrentPage,
        //            users.PageSize,
        //            users.TotalCount,
        //            users.TotalPages,
        //            users.HasPreviousPage,
        //            users.HasNextPage
        //        );

        //        var result = new
        //        {
        //            users,
        //            users.CurrentPage,
        //            users.PageSize,
        //            users.TotalCount,
        //            users.TotalPages,
        //            users.HasPreviousPage,
        //            users.HasNextPage
        //        };

        //        var successResult = Result.Success(result);
        //        return Ok(successResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }
        //}




    [AllowAnonymous]
        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddNewUser([FromBody] AddNewUserCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddNewUserValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }

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

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                }

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


        [HttpPatch("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusCommand command)
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


        [HttpPut("UserChangePassword")]
        public async Task<IActionResult> UserChangePassword([FromBody] UserChangePasswordCommand command)
        {
            try
            {

                var validationResult = await _validatorHandler.UserChangePasswordValidator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);

                }

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


        [HttpPut("UserResetPassword")]
        public async Task<IActionResult> UserResetPassword([FromBody] UserResetPasswordCommand command)
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

        //[HttpPut("update-profile")]
        //public async Task<IActionResult> UpdateProfilePic([FromForm] UpdateProfilePicCommand command)
        //{
        //    try
        //    {

        //        if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
        //        {
        //            command.UserId = userId;
        //        }

        //        var result = await _mediator.Send(command);
        //        if (result.IsFailure)
        //        {
        //            return BadRequest(result);
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }

        //}

        [HttpGet("GetUserByPermission")]
        public async Task<IActionResult> GetUserByPermission([FromQuery] GetUserByPermissionQuery query)
        {
            try
            {
                //var cacheKey = $"users-{query.PageNumber}-{query.PageSize}";
                //var cachedUsers = await _cacheService.GetCacheAsync(cacheKey);

                //if (cachedUsers != null)
                //{
                //    return Ok(Result.Success(cachedUsers));
                //}

                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasPreviousPage,
                    users.HasNextPage
                );

                var result = new
                {
                    users,
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasPreviousPage,
                    users.HasNextPage
                };

                //await _cacheService.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
