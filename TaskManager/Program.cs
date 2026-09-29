
using Mapster;
using TaskManager.Api.Configurations;
using TaskManager.Application.Configuration;
using TaskManager.Application.Mappings;
using TaskManager.Infra.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//Application and Infra services

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddRateLimitConfiguration();
builder.Services.AddCorsConfiguration(builder.Configuration);

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

TypeAdapterConfig.GlobalSettings.Scan(typeof(TaskMapping).Assembly);

var app = builder.Build();
app.UseExceptionHandler();


app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapGet("/health", () => Results.Ok("Healthy"));

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();