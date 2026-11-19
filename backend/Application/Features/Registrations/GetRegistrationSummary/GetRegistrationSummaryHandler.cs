
namespace Application.Features.Registrations.GetRegistrationSummary;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationSummaryHandler
    : IQueryHandler<GetRegistrationSummaryQuery, Result<GetRegistrationSummaryResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetRegistrationSummaryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetRegistrationSummaryResponse>> HandleAsync(
        GetRegistrationSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var registrations = _dbContext.Registrations.AsNoTracking();

        var totalRegistrations = await registrations.CountAsync(cancellationToken);

        var pending = await registrations.CountAsync(
            r => r.Status == RegistrationStatus.Pending,
            cancellationToken);

        var confirmed = await registrations.CountAsync(
            r => r.Status == RegistrationStatus.Confirmed,
            cancellationToken);

        var checkedIn = await registrations.CountAsync(
            r => r.Status == RegistrationStatus.CheckedIn,
            cancellationToken);

        var cancelled = await registrations.CountAsync(
            r => r.Status == RegistrationStatus.Cancelled,
            cancellationToken);

        // Exclude cancelled registrations from money totals.
        var activeRegistrations = registrations
            .Where(r => r.Status != RegistrationStatus.Cancelled);

        var totalFees = await activeRegistrations
            .SumAsync(r => (decimal?)r.RegistrationFee, cancellationToken) ?? 0m;

        var totalPayments = await activeRegistrations
            .SumAsync(r => (decimal?)r.AmountPaid, cancellationToken) ?? 0m;

        var uncollectedPayments = totalFees - totalPayments;

        var response = new GetRegistrationSummaryResponse(
            totalRegistrations,
            confirmed,
            checkedIn,
            pending,
            cancelled,
            totalFees,
            totalPayments,
            uncollectedPayments);

        return Result<GetRegistrationSummaryResponse>.Success(response);
    }
}
