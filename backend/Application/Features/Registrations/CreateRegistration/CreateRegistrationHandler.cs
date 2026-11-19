
namespace Application.Features.Registrations.CreateRegistration;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class CreateRegistrationHandler
    : ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>>
{
    private readonly IAppDbContext _dbContext;

    public CreateRegistrationHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreateRegistrationResponse>> HandleAsync(
        CreateRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        var plateNumber = command.PlateNumber.Trim().ToUpperInvariant();

        var plateExists = await _dbContext.Registrations
            .AnyAsync(r => r.PlateNumber == plateNumber, cancellationToken);

        if (plateExists)
        {
            return Result<CreateRegistrationResponse>.Failure(
                Error.Conflict(
                    "Registration.Conflict.DuplicatePlateNumber",
                    "This plate number is already registered."));
        }

        var registrationFee = command.RegistrationType switch
        {
            RegistrationType.Exhibitor => 1500m,
            RegistrationType.Competitor => 2500m,
            RegistrationType.VIP => 5000m,
            _ => 0m
        };

        if (command.AmountPaid > registrationFee)
        {
            return Result<CreateRegistrationResponse>.Failure(
                Error.Validation(
                    "Registration.Validation.InvalidAmountPaid",
                    "Amount paid cannot exceed the registration fee."));
        }

        var lastRegistration = await _dbContext.Registrations
            .OrderByDescending(r => r.RegistrationNumber)
            .Select(r => r.RegistrationNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = 1;

        if (!string.IsNullOrWhiteSpace(lastRegistration) &&
            int.TryParse(lastRegistration.Split('-').Last(), out var lastNumber))
        {
            nextNumber = lastNumber + 1;
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = $"CS27-{nextNumber:D4}",
            RegistrationDate = DateTime.UtcNow,
            ParticipantName = command.ParticipantName.Trim(),
            ContactNumber = command.ContactNumber.Trim(),
            Email = command.Email.Trim(),
            Address = command.Address.Trim(),
            CarMakeModel = command.CarMakeModel.Trim(),
            PlateNumber = plateNumber,
            RegistrationType = command.RegistrationType,
            Status = RegistrationStatus.Pending,
            RegistrationFee = registrationFee,
            AmountPaid = command.AmountPaid
        };

        _dbContext.Registrations.Add(registration);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var paymentStatus = registration.AmountPaid == 0
            ? "Unpaid"
            : registration.AmountPaid < registration.RegistrationFee
                ? "Partial"
                : "Paid";

        var response = new CreateRegistrationResponse(
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

        return Result<CreateRegistrationResponse>.Success(response);
    }
}
