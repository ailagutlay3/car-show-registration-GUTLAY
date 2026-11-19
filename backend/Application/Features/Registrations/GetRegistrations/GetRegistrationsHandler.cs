
namespace Application.Features.Registrations.GetRegistrations;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationsHandler
    : IQueryHandler<GetRegistrationsQuery, Result<GetRegistrationsResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetRegistrationsHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetRegistrationsResponse>> HandleAsync(
        GetRegistrationsQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Registration> registrations =
            _dbContext.Registrations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.RegistrationType) &&
            Enum.TryParse<RegistrationType>(
                query.RegistrationType, true, out var type))
        {
            registrations = registrations.Where(
                r => r.RegistrationType == type);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<RegistrationStatus>(
                query.Status, true, out var status))
        {
            registrations = registrations.Where(
                r => r.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.ParticipantName))
        {
            var participantName = query.ParticipantName.Trim();

            registrations = registrations.Where(r =>
                EF.Functions.Like(
                    r.ParticipantName,
                    $"%{participantName}%"));
        }

        var totalCount = await registrations.CountAsync(cancellationToken);

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var records = await registrations
            .OrderByDescending(r => r.RegistrationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = records.Select(r => new RegistrationListItem(
            r.Id,
            r.RegistrationNumber,
            r.RegistrationDate,
            r.ParticipantName,
            r.ContactNumber,
            r.Email,
            r.Address,
            r.CarMakeModel,
            r.PlateNumber,
            r.RegistrationType,
            r.Status,
            r.RegistrationFee,
            r.AmountPaid,
            r.AmountPaid == 0 ? "Unpaid" :
                r.AmountPaid < r.RegistrationFee ? "Partial" : "Paid"
        )).ToList();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize);

        var response = new GetRegistrationsResponse(
            items,
            page,
            pageSize,
            totalCount,
            totalPages);

        return Result<GetRegistrationsResponse>.Success(response);
    }
}
