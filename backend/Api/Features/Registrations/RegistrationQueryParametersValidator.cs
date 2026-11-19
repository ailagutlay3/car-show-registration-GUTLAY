namespace Api.Features.Registrations;

using FluentValidation;
using Domain.Entities;

public sealed class RegistrationQueryParametersValidator
    : AbstractValidator<RegistrationQueryParameters>
{
    public RegistrationQueryParametersValidator()
    {
        RuleFor(x => x.Page)
            .Must(value => value is null || value >= 1)
            .WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .Must(value => value is null || (value >= 1 && value <= 50))
            .WithMessage("PageSize must be between 1 and 50.");

        RuleFor(x => x.ParticipantName)
            .MaximumLength(150);

        RuleFor(x => x.Type)
            .Must(value => string.IsNullOrWhiteSpace(value) ||
                Enum.TryParse<RegistrationType>(value, true, out _))
            .WithMessage("Type must be Exhibitor, Competitor, or VIP.");

        RuleFor(x => x.Status)
            .Must(value => string.IsNullOrWhiteSpace(value) ||
                Enum.TryParse<RegistrationStatus>(value, true, out _))
            .WithMessage("Status must be Pending, Confirmed, CheckedIn, or Cancelled.");
    }
}