using FluentValidation;
using MakeItSimple.WebApi.Common.Behavior;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.AspNetCore.Http.Connections;
using MakeItSimple.WebApi.Common.SignalR;
using System.Data;
using Microsoft.Data.SqlClient;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
//using MakeItSimple.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("Testing");
builder.Services.AddDbContext<MisDbContext>(x =>
x.UseSqlServer(connectionString, sqlOptions => sqlOptions.CommandTimeout(180))
    .UseSnakeCaseNamingConvention()
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)

);

var redisConnectionString = config.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});


builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
builder.Services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddControllers( options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();

    options.Filters.Add(new AuthorizeFilter(policy));

}).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<ValidatorHandler>();
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddScoped<TransformUrl>();
builder.Services.AddScoped<ContentType>();
builder.Services.AddScoped<TimerControl>();
//builder.Services.AddScoped<IHubCaller, HubCaller>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RDF MakeItSimple(TicketSystem) API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});

builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwtOptions =>
    {
        var key = builder.Configuration.GetValue<string>("JwtConfig:Key");
        var keyBytes = Encoding.ASCII.GetBytes(key);
        jwtOptions.SaveToken = true;
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),                          
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"]
        };

        jwtOptions.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/notification-hub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

    });



builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMemoryCache();
builder.Services.AddLazyCache();
//builder.Services.AddSignalR();



builder.Services.Configure<CloudinaryOption>(config.GetSection("Cloudinary"));

const string clientPermission = "_clientPermission";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: clientPermission, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//kestrell
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // HTTP
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});


builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors(clientPermission);
//app.MapHub<CacheHub>("/cacheNotificationHub");

app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    //endpoints.MapHub<NotificationHub>("/notification-hub", options =>
    //{
    //    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
    //    options.CloseOnAuthenticationExpiration = true;
    //});

});

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MisDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

//using (var scope = app.Services.CreateScope())
//{
//    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
//    //await cacheService.GetTicketOnHolds();
//    //await cacheService.GetClosingTickets();
//    await cacheService.GetOpenTickets();
//    //await cacheService.GetOpenTicketsChannel();
//    //await cacheService.GetTransferTicketConcerns();

//}


app.Run();
