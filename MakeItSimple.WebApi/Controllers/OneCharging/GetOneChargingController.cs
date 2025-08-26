using Azure;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneBusinessUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneCharging;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneCompany;

//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneCompany;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneDepartment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneLocation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneUnit;


namespace MakeItSimple.WebApi.Controllers.OneCharging
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetOneChargingController : ControllerBase
    {

        public readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public GetOneChargingController(IMediator mediator, ValidatorHandler validatorHandler)
        {

            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }



        [HttpGet("page")]
        public async Task<IActionResult> GetOneCharging([FromQuery] GetOneChargingQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpGet("page_business_unit")]
        public async Task<IActionResult> GetOneBusinessUnit([FromQuery] GetOneBusinessUnitQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpGet("page_company")]
        public async Task<IActionResult> GetOneCompany([FromQuery] GetOneCompanyQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpGet("page_department")]
        public async Task<IActionResult> GetOneDepartment([FromQuery] GetOneDepartmentQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpGet("page_location")]
        public async Task<IActionResult> GetOneLocation([FromQuery] GetOneLocationQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("page_subunit")]
        public async Task<IActionResult> GetOneSubunit([FromQuery] GetOneSubUnitQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpGet("page_unit")]
        public async Task<IActionResult> GetOneUnit([FromQuery] GetOneUnitQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

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
