using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Authentication;
//using MakeItSimple.WebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Authentication.LogoutAuthentication;

namespace MakeItSimple.WebApi.Controllers.Authentication
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly TokenGenerator _tokenGenerator;
        private readonly MisDbContext _context;
        //private readonly IHubCaller _hubCaller;

        public AuthenticationController(IMediator mediator, TokenGenerator tokenGenerator, MisDbContext context)
        {
            _mediator = mediator;
            _tokenGenerator = tokenGenerator;
            _context = context;
            //_hubCaller = hubCaller;
        }

        [AllowAnonymous]
        [HttpPost("AuthenticateUser")]
        public async Task<ActionResult<AuthenticateUser.AuthenticateUserResult>> AuthenticateUser(AuthenticateUser.AuthenticateUserQuery request)
        {
            try
            {
                Guid eyow = new Guid("D99D6DC5-3197-4F2E-94B7-F409F746A9BB");
                var result = await _mediator.Send(request);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                //await _hubCaller.SendNotificationAsync(eyow, "LoggedIn", "Maybe");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        //[Authorize]
        //[HttpPost("LogoutAuthentication")]
        //public async Task<IActionResult> LogoutAuthentication()
        //{

        //    try
        //    {
        //        if (User.Identity is ClaimsIdentity identity)
        //        {
        //            var userIdClaim = identity.FindFirst("id");

        //            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        //            {

        //                var User = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        //                var shortLivedToken = _tokenGenerator.GenerateShortLivedToken(User);

        //                var command = new LogoutAuthentication.LogoutAuthenticationCommand
        //                {
        //                    Id = User.Id,
        //                    Token = shortLivedToken
        //                };

        //                var result = await _mediator.Send(command);

        //                if (result.IsFailure)
        //                {
        //                    return BadRequest(result);
        //                }

        //                return Ok(result);
        //            }
        //        }

        //        // Handle the case where the user identity or "id" claim is not available
        //        return BadRequest("Invalid user identity or missing 'id' claim.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }

        //}

    }
}
