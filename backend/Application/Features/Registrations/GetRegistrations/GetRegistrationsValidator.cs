
namespace Application.Features.Registrations.GetRegistrations;

using FluentValidation;

public sealed class GetRegistrationsValidator : AbstractValidator<GetRegistrationsQuery>
{
    public GetRegistrationsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.ParticipantName)
            .MaximumLength(150)
            .WithMessage("Participant name must not exceed 150 characters.");

        RuleFor(x => x.RegistrationType)
            .Must(value => string.IsNullOrWhiteSpace(value) ||
                Enum.TryParse<Domain.Entities.RegistrationType>(value, true, out _))
            .WithMessage("Invalid registration type. Allowed values: Exhibitor, Competitor, VIP.");

        RuleFor(x => x.Status)
            .Must(value => string.IsNullOrWhiteSpace(value) ||
                Enum.TryParse<Domain.Entities.RegistrationStatus>(value, true, out _))
            .WithMessage("Invalid registration status. Allowed values: Pending, Confirmed, CheckedIn, Cancelled.");
    }
}
