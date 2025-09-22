using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup.GetCategoryConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup.UpdateCategoryConcernStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup.UpsertCategoryConcern;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_One.CategoryConcernController
{
    [Route("api/categoryConcern")]
    [ApiController]
    public class CategoryConcernController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryConcernController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> UpsertCategoryConcern([FromBody] UpsertCategoryConcernCommand command)
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
        [HttpGet("page")]
        public async Task<IActionResult> GetCategoryConcern([FromQuery] GetCategoryConcernQuery query )
        {
            try
            {
                var category = await _mediator.Send(query);

                Response.AddPaginationHeader(

                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasPreviousPage,
                category.HasNextPage

                );

                var result = new
                {
                    category,
                    category.CurrentPage,
                    category.PageSize,
                    category.TotalCount,
                    category.TotalPages,
                    category.HasPreviousPage,
                    category.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateCategoryConcernStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateCategoryConcernStatusCommand
                {
                    Id = id
                };
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
