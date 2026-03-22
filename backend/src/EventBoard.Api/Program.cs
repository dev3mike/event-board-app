using EventBoard.Api.Middleware;
using EventBoard.Api.DTOs;
using EventBoard.Application;
using EventBoard.Infrastructure;
using EventBoard.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

ConfigureControllersJsonOptions(builder);
ConfigureValidationErrorResponses(builder.Services);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.RequestQuery
        | HttpLoggingFields.ResponseStatusCode
        | HttpLoggingFields.Duration;
});

// Application + Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

if (!builder.Environment.IsProduction())
{
    ConfigureSwagger(builder);
}

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
p.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Apply migrations and seed data on startup
// TODO: Move to a standalone migration script
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db, timeProvider);
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpLogging();

if (!app.Environment.IsProduction())
{
    UseSwaggerUi(app);
}

app.UseCors();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    if (!app.Environment.IsProduction())
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var baseUrl = app.Urls.FirstOrDefault();
        logger.LogInformation("Swagger UI available at {SwaggerUrl}", $"{baseUrl?.TrimEnd('/')}/swagger");
    }
});

app.Run();

static void ConfigureControllersJsonOptions(WebApplicationBuilder builder)
{
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    });
}

// Return a consistent ApiErrorDto shape for DataAnnotations validation failures
static void ConfigureValidationErrorResponses(IServiceCollection services)
{
    services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => JsonNamingPolicy.SnakeCaseLower.ConvertName(kvp.Key),
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            return new BadRequestObjectResult(new ApiErrorDto
            {
                Code = "ValidationFailed",
                Message = "One or more validation errors occurred.",
                ValidationErrors = errors
            });
        };
    });
}

static void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Event Board API",
            Version = "v1",
            Description = "REST API for browsing events and registering participants."
        });
        c.EnableAnnotations();

        var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
        c.IncludeXmlComments(apiXmlPath);

        var applicationAssembly = typeof(EventBoard.Application.DependencyInjection).Assembly;
        var applicationXmlFile = $"{applicationAssembly.GetName().Name}.xml";
        var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
        c.IncludeXmlComments(applicationXmlPath);
    });
}

static void UseSwaggerUi(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Board API v1"));
}

// Exposed for the test project
public partial class Program { }
