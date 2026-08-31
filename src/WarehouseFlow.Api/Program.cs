using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Api.Middleware;
using WarehouseFlow.Application;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Infrastructure;
using WarehouseFlow.Infrastructure.Data;
using WarehouseFlow.Infrastructure.Identity;
using WarehouseFlow.Infrastructure.Implementations;
using WarehouseFlow.Infrastructure.BackgroundServices;
using WarehouseFlow.Domain.Enum;

var builder = WebApplication.CreateBuilder(args);

// Add command-line args to configuration so IdentitySeeder can access them
builder.Configuration.AddCommandLine(args);

// Register layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHostedService<ReservationCleanupService>();

builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.text", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System
            .Text
            .Json
            .JsonNamingPolicy
            .CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = context =>
        {
            var validEmployeeRoles = Enum
                .GetNames<UserRole>()
                .Where(role => role != nameof(UserRole.Customer));
            var roleError =
                $"Role must be one of: {string.Join(", ", validEmployeeRoles)}.";

            var errors = context
                .ModelState.Where(kv => kv.Value?.Errors.Count > 0)
                .SelectMany(kv => kv.Value!.Errors.Select(error => new
                {
                    Key = kv.Key,
                    Message = error.ErrorMessage,
                    IsJsonError = error.Exception is JsonException
                        || error.Exception?.InnerException is JsonException,
                }))
                .Where(error =>
                    !(
                        error.Key.Equals("employeeUserDto", StringComparison.OrdinalIgnoreCase)
                        && error.Message.Contains("field is required", StringComparison.OrdinalIgnoreCase)
                    )
                )
                .Select(error =>
                    error.IsJsonError
                    && error.Key.EndsWith("role", StringComparison.OrdinalIgnoreCase)
                        ? roleError
                        : error.Message
                )
                .Distinct()
                .ToList();

            return new BadRequestObjectResult(
                ApiResponse<object>.FailureResult(
                    "One or more validation errors occurred.",
                    errors,
                    StatusCodes.Status400BadRequest
                )
            );
        }
    );

// Add Jwt
var jwtSecret =
    builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.FailureResult(
                    "Authentication is required. Provide a valid bearer token.",
                    statusCode: StatusCodes.Status401Unauthorized
                );

                return context.Response.WriteAsJsonAsync(response);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.FailureResult(
                    "You do not have permission to perform this action.",
                    statusCode: StatusCodes.Status403Forbidden
                );

                return context.Response.WriteAsJsonAsync(response);
            },
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a JWT token. Swagger will send it as: Bearer {token}",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>(),
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
    await IdentitySeeder.SeedSuperAdminAsync(scope.ServiceProvider, app.Configuration);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
