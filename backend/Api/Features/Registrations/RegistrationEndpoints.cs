
namespace Api.Features.Registrations;

using Api.Common.Extensions;
using Api.Common.Filters;
using Application.Abstractions.Messaging;
using Application.Features.Registrations.CreateRegistration;
using Application.Features.Registrations.GetRegistrations;
using Application.Features.Registrations.GetRegistrationById;
using Application.Features.Registrations.GetRegistrationSummary;
using Domain.Common;

public static class RegistrationEndpoints
{
    public static void MapRegistrationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/registrations")
            .WithTags("Registrations");

        // POST: Create a new registration
        group.MapPost("/", async (
            CreateRegistrationCommand command,
            ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ToProblemDetails();
            }

            return Results.Created(
                $"/api/registrations/{result.Value!.Id}",
                result.Value);
        })
        .AddEndpointFilter<ValidationFilter<CreateRegistrationCommand>>()
        .WithName("CreateRegistration")
        .WithSummary("Create a new car show registration");

        // GET: Registration summary
        group.MapGet("/summary", async (
            IQueryHandler<GetRegistrationSummaryQuery, Result<GetRegistrationSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(
                new GetRegistrationSummaryQuery(),
                cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        })
        .WithName("GetRegistrationSummary")
        .WithSummary("Get dashboard KPI totals");

        // GET: All registrations with filters and pagination
        group.MapGet("/", async (
            [AsParameters] RegistrationQueryParameters parameters,
            IQueryHandler<GetRegistrationsQuery, Result<GetRegistrationsResponse>> handler,
            CancellationToken cancellationToken) =>
        {
           var query = new GetRegistrationsQuery(
    parameters.Page ?? 1,
    parameters.PageSize ?? 10,
    parameters.Type,
    parameters.Status,
    parameters.ParticipantName);
            var result = await handler.HandleAsync(query, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        })
        .AddEndpointFilter<ValidationFilter<RegistrationQueryParameters>>()
        .WithName("GetRegistrations")
        .WithSummary("Get paginated and filtered registrations");

        // GET: Registration by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRegistrationByIdQuery, Result<GetRegistrationByIdResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRegistrationByIdQuery(id);
            var result = await handler.HandleAsync(query, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        })
        .WithName("GetRegistrationById")
        .WithSummary("Get a registration by ID");
    }
}
