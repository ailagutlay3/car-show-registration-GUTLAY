namespace Application.Features.Registrations.GetRegistrationById;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationByIdHandler
    : IQueryHandler<GetRegistrationByIdQuery, Result<GetRegistrationByIdResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetRegistrationByIdHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetRegistrationByIdResponse>> HandleAsync(
        GetRegistrationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var registration = await _dbContext.Registrations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == query.Id, cancellationToken);

        if (registration is null)
        {
            return Result<GetRegistrationByIdResponse>.Failure(
                Error.NotFound(
                    "Registration.NotFound",
                    "Registration was not found."));
        }

        var paymentStatus = registration.AmountPaid == 0
            ? "Unpaid"
            : registration.AmountPaid < registration.RegistrationFee
                ? "Partial"
                : "Paid";

        var response = new GetRegistrationByIdResponse(
            registration.Id,
            registration.RegistrationNumber,
            registration.RegistrationDate,
            registration.ParticipantName,
            registration.ContactNumber,
            registration.Email,
            registration.Address,
            registration.CarMakeModel,
            registration.PlateNumber,
            registration.RegistrationType,
            registration.Status,
            registration.RegistrationFee,
            registration.AmountPaid,
            paymentStatus);

        return Result<GetRegistrationByIdResponse>.Success(response);
    }
}
