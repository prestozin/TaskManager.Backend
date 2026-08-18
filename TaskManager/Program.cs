using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using TaskManager.Api.Configurations;
using TaskManager.Application.Configuration;
using TaskManager.Application.Mappings;
using TaskManager.Application.Validators;
using TaskManager.Infra.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Application and Infra services

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


TypeAdapterConfig.GlobalSettings.Scan(typeof(TaskMapping).Assembly);

var app = builder.Build();

app.UseCors("Angular");

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();