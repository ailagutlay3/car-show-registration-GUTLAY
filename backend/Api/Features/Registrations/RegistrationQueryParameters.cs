
namespace Api.Features.Registrations;

public sealed class RegistrationQueryParameters
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? ParticipantName { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}