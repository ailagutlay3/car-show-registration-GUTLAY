
namespace Api.Features.Registrations;

using Application.Features.Registrations.CreateRegistration;
using Domain.Entities;

public sealed record CreateRegistrationRequest(
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    decimal AmountPaid
)
{
    public CreateRegistrationCommand ToCommand() =>
        new(
            ParticipantName,
            ContactNumber,
            Email,
            Address,
            CarMakeModel,
            PlateNumber,
            RegistrationType,
            AmountPaid);
}
