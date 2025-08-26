using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ApproverUserSetup.AddApproverUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ApproverUserSetup.GetApproverUser;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_One.ApproverUserController
{
    [Route("api/approveruser")]
    [ApiController]
    public class ApproverUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ApproverUserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("AddApproverUsers")]
        public async Task<IActionResult> AddApproverUsers([FromBody] AddApproverUserCommand command)
        {
            try
            {
                if(command.ApproverId == Guid.Empty)
                {
                    return BadRequest("ApproverId cannot be empty");
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


        [HttpGet("GetApproverUsers")]
        public async Task<IActionResult> GetApproverUsers([FromQuery] GetApproverUserQuery query)
        {
            var approver = await _mediator.Send(query);

            Response.AddPaginationHeader(

            approver.CurrentPage,
            approver.PageSize,
            approver.TotalCount,
            approver.TotalPages,
            approver.HasPreviousPage,
            approver.HasNextPage

            );

            var result = new
            {
                approver,
                approver.CurrentPage,
                approver.PageSize,
                approver.TotalCount,
                approver.TotalPages,
                approver.HasPreviousPage,
                approver.HasNextPage
            };

            var successResult = Result.Success(result);
            return Ok(successResult);
        }
    }
}
