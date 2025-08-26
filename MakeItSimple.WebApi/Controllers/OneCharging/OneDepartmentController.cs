//using MakeItSimple.WebApi.Common;
//using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
//using MakeItSimple.WebApi.Models.OneCharging;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace MakeItSimple.WebApi.Controllers.OneCharging
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OneDepartmentController : ControllerBase
//    {
//        public readonly IMediator _mediator;
//        private readonly IConfiguration _config;

//        public OneDepartmentController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpPost("sync")]
//        //[ApiKeyAuth]
//        public async Task<IActionResult> Sync()
//        {

//            var result = await _mediator.Send(new ImportDepartmentCommand());
//            return result.IsFailure ? BadRequest(result) : Ok(result);

//        }


//        public class ImportDepartmentCommand : IRequest<Result> { }

//        public class Handler : IRequestHandler<ImportDepartmentCommand, Result>
//        {
//            public readonly MisDbContext _storeContext;
//            public readonly IHttpClientFactory _httpClientFactory;

//            public Handler(MisDbContext storeContext, IHttpClientFactory httpClientFactory)
//            {
//                _storeContext = storeContext;
//                _httpClientFactory = httpClientFactory;
//            }

//            public async Task<Result> Handle(ImportDepartmentCommand request, CancellationToken cancellationToken)
//            {
//                var client = _httpClientFactory.CreateClient();
//                var apiEndPoint = "https://api-one.rdfmis.com/api/departments_api?per_page=10&page=1&pagination=none&status=active";

//                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiEndPoint);
//                httpRequest.Headers.Add("API_KEY", "hello world!");

//                var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
//                if (!httpResponse.IsSuccessStatusCode)
//                {
//                    return Result.Failure(new Error("Failed to retrieve data from the source API", "NOT_FOUND"));
//                }
//                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
//                if (response == null || response.Data == null)
//                {
//                    return Result.Failure(new Error("Failed to parse response from the source API.", "INVALID_RESPONSE"));
//                }

//                foreach (var item in response.Data)
//                {
//                    var existing = await _storeContext.OneDepartments
//                        .FirstOrDefaultAsync(x => x.sync_id == item.Id, cancellationToken);

//                    if (existing == null)
//                    {
//                        var charging = new OneDepartment
//                        {
//                            sync_id = item.Id,
//                            department_code = item.code,
//                            department_name = item.name,
//                            deleted_at = item.deleted_at,
//                            IsActive = item.deleted_at != null ? false : true,
//                            DateAdded = item.created_at,
//                            UpdatedAt = item.updated_at,
//                        };

//                        await _storeContext.OneDepartments.AddAsync(charging, cancellationToken);
//                    }
//                    else if (existing.UpdatedAt != item.updated_at)
//                    {

//                        existing.department_code = item.code;
//                        existing.department_name = item.name;
//                        existing.deleted_at = item.deleted_at;
//                        existing.IsActive = item.deleted_at != null ? false : true;
//                        existing.UpdatedAt = item.updated_at;
//                    }


//                }
//                await _storeContext.SaveChangesAsync(cancellationToken);
//                return Result.Success("Department synced successfully.");
//            }

//        }

//        public class ApiResponse
//        {
//            public int Status { get; set; }
//            public string Message { get; set; } = string.Empty;
//            public List<OneDepartmentDto>? Data { get; set; }
//        }

//        public class OneDepartmentDto
//        {
//            public int Id { get; set; }
//            public string code { get; set; } = string.Empty;
//            public string name { get; set; } = string.Empty;
//            public DateTime? created_at { get; set; }
//            public DateTime? updated_at { get; set; }
//            public DateTime? deleted_at { get; set; }
//        }
//    }
//}
