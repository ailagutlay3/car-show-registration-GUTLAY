namespace Application.Features.Registrations.GetRegistrations;

using Application.Abstractions.Messaging;
using Domain.Common;

public sealed record GetRegistrationsQuery(
    int Page = 1,
    int PageSize = 10,
    string? RegistrationType = null,
    string? Status = null,
    string? ParticipantName = null
) : IQuery<Result<GetRegistrationsResponse>>;
