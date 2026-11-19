
namespace Application.Features.Registrations.GetRegistrations;

using Domain.Entities;

public sealed record RegistrationListItem(
    Guid Id,
    string RegistrationNumber,
    DateTime RegistrationDate,
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    RegistrationStatus Status,
    decimal RegistrationFee,
    decimal AmountPaid,
    string PaymentStatus
);

public sealed record GetRegistrationsResponse(
    IReadOnlyList<RegistrationListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
