using Api.Common.Converters;
using System.Text.Json.Serialization;

using Api.Common.Filters;
using Api.Features.Registrations;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Application.Features.Registrations.CreateRegistration;
using Application.Features.Registrations.GetRegistrations;
using Application.Features.Registrations.GetRegistrationById;
using Application.Features.Registrations.GetRegistrationSummary;

using Domain.Common;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database - SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=carshow.db"));

builder.Services.AddScoped<IAppDbContext>(serviceProvider =>
    serviceProvider.GetRequiredService<AppDbContext>());

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(IAppDbContext).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(RegistrationQueryParametersValidator).Assembly);

// Validation filters
builder.Services.AddScoped(typeof(ValidationFilter<>));

// Command handlers
builder.Services.AddScoped<
    ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>>,
    CreateRegistrationHandler>();

// Query handlers
builder.Services.AddScoped<
    IQueryHandler<GetRegistrationsQuery, Result<GetRegistrationsResponse>>,
    GetRegistrationsHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetRegistrationByIdQuery, Result<GetRegistrationByIdResponse>>,
    GetRegistrationByIdHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetRegistrationSummaryQuery, Result<GetRegistrationSummaryResponse>>,
    GetRegistrationSummaryHandler>();

// CORS - React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Insert(
        0,
        new RegistrationTypeJsonConverter());

    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});
// OpenAPI and Scalar
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Car Show Registration API";
        document.Info.Description = "API for the Car Show Registration System";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Apply database migrations and seed sample data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAsync(dbContext);
}

// CORS middleware
app.UseCors("ReactFrontend");

// API documentation
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Root endpoint
app.MapGet("/", () =>
    "Welcome to the Car Show Registration API! Go to /scalar/v1 for API documentation.");

// Registration endpoints
app.MapRegistrationEndpoints();

app.Run();
