using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Depots.DTOs;
using LastMile.TMS.Application.Depots.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Depots.Queries.Handlers;

public class GetDepotByIdQueryHandler(IAppDbContext db) : IRequestHandler<GetDepotByIdQuery, DepotDto?>
{
    public async Task<DepotDto?> Handle(GetDepotByIdQuery request, CancellationToken cancellationToken)
    {
        var depot = await db.Depots
            .Include(d => d.Address)
            .Include(d => d.OperatingHours)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (depot is null)
            return null;

        return new DepotDto(
            depot.Id,
            depot.Name,
            depot.Address is not null
                ? new AddressDto(
                    depot.Address.Street1,
                    depot.Address.Street2,
                    depot.Address.City,
                    depot.Address.State,
                    depot.Address.PostalCode,
                    depot.Address.CountryCode,
                    depot.Address.IsResidential,
                    depot.Address.ContactName,
                    depot.Address.CompanyName,
                    depot.Address.Phone,
                    depot.Address.Email)
                : null,
            depot.OperatingHours.Select(h => new OperatingHoursDto(h.DayOfWeek, h.OpenTime, h.ClosedTime, h.IsClosed)).ToList(),
            depot.IsActive,
            depot.CreatedAt,
            depot.LastModifiedAt
        );
    }
}
