using Azure.Core;
using DocumentFormat.OpenXml.EMMA;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Controllers.Authentication;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Controllers.OneCharging
{

    [Route("api/charging")]
    [ApiController]
    public class OneChargingController : ControllerBase
    {

        public readonly IMediator _mediator;


        public OneChargingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sync")]
        //[ApiKeyAuth]
        public async Task<IActionResult> Sync()
        {

            var result = await _mediator.Send(new ImportChargingCommand());
            return result.IsFailure ? BadRequest(result) : Ok(result);

        }


        public class ImportChargingCommand : IRequest<Result> { }

        public class Handler : IRequestHandler<ImportChargingCommand, Result>
        {
            public readonly MisDbContext _storeContext;
            public readonly IHttpClientFactory _httpClientFactory;

            public Handler(MisDbContext storeContext, IHttpClientFactory httpClientFactory)
            {
                _storeContext = storeContext;
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Result> Handle(ImportChargingCommand request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient();
                var apiEndPoint = "https://api-one.rdfmis.com/api/charging_api?per_page=10&page=1&pagination=none";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiEndPoint);
                httpRequest.Headers.Add("API_KEY", "hello world!");

                var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return Result.Failure(new Error("Failed to retrieve data from the source API", "NOT_FOUND"));
                }
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
                if (response == null || response.Data == null)
                {
                    return Result.Failure(new Error("Failed to parse response from the source API.", "INVALID_RESPONSE"));
                }

                foreach (var item in response.Data)
                {
                    var existing = await _storeContext.OneChargings
                        .FirstOrDefaultAsync(x => x.sync_id == item.Id, cancellationToken);

                    if (existing == null)
                    {
                        var charging = new OneChargingMIS
                        {
                            sync_id = item.Id,
                            code = item.code,
                            name = item.name,
                            company_id = item.company_id,
                            company_code = item.company_code,
                            company_name = item.company_name,
                            business_unit_id = item.business_unit_id,
                            business_unit_code = item.business_unit_code,
                            business_unit_name = item.business_unit_name,
                            department_id = item.department_id,
                            department_code = item.department_code,
                            department_name = item.department_name,
                            department_unit_id = item.unit_id,
                            department_unit_code = item.unit_code,
                            department_unit_name = item.unit_name,
                            sub_unit_id = item.sub_unit_id,
                            sub_unit_code = item.sub_unit_code,
                            sub_unit_name = item.sub_unit_name,
                            location_id = item.location_id,
                            location_code = item.location_code,
                            location_name = item.location_name,
                            deleted_at = item.deleted_at,
                            IsActive = item.deleted_at != null ? false : true,
                            DateAdded = item.created_at,
                            UpdatedAt = item.updated_at,
                        };

                        await _storeContext.OneChargings.AddAsync(charging, cancellationToken);
                    }
                    else if (existing.UpdatedAt != item.updated_at)
                    {
                        existing.code = item.code;
                        existing.name = item.name;
                        existing.company_id = item.company_id;
                        existing.company_code = item.company_code;
                        existing.company_name = item.company_name;
                        existing.business_unit_id = item.business_unit_id;
                        existing.business_unit_code = item.business_unit_code;
                        existing.business_unit_name = item.business_unit_name;
                        existing.department_id = item.department_id;
                        existing.department_code = item.department_code;
                        existing.department_name = item.department_name;
                        existing.department_unit_id = item.unit_id;
                        existing.department_unit_code = item.unit_code;
                        existing.department_unit_name = item.unit_name;
                        existing.sub_unit_id = item.sub_unit_id;
                        existing.sub_unit_code = item.sub_unit_code;
                        existing.sub_unit_name = item.sub_unit_name;
                        existing.location_id = item.location_id;
                        existing.location_code = item.location_code;
                        existing.location_name = item.location_name;
                        existing.deleted_at = item.deleted_at;
                        existing.IsActive = item.deleted_at != null ? false : true;
                        existing.UpdatedAt = item.updated_at;
                    }


                }
                await _storeContext.SaveChangesAsync(cancellationToken);
                return Result.Success("Charging synced successfully.");
            }

        }

        public class ApiResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<ChargingDto>? Data { get; set; }
        }

        public class ChargingDto
        {
            public int Id { get; set; }
            public string code { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public int? company_id {  get; set; }
            public string company_code { get; set; } = string.Empty;
            public string company_name { get; set; } = string.Empty;
            public int? business_unit_id {  get; set; } 
            public string business_unit_code { get; set; } = string.Empty;
            public string business_unit_name { get; set; } = string.Empty;
            public int? department_id {  get; set; }
            public string department_code { get; set; } = string.Empty;
            public string department_name { get; set; } = string.Empty;
            public int? unit_id {  get; set; }
            public string unit_code { get; set; } = string.Empty;
            public string unit_name { get; set; } = string.Empty;
            public int? sub_unit_id { get; set; }
            public string sub_unit_code { get; set; } = string.Empty;
            public string sub_unit_name { get; set; } = string.Empty;
            public int? location_id {  get; set; }
            public string location_code { get; set; } = string.Empty;
            public string location_name { get; set; } = string.Empty;
            public DateTime? created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public DateTime? deleted_at { get; set; }
        }
    }
}
