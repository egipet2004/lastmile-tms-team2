using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Depots.DTOs;
using LastMile.TMS.Application.Depots.Queries;
using LastMile.TMS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Depots.Queries.Handlers;

public class GetAllDepotsQueryHandler(IAppDbContext db) : IRequestHandler<GetAllDepotsQuery, List<DepotDto>>
{
    public async Task<List<DepotDto>> Handle(GetAllDepotsQuery request, CancellationToken cancellationToken)
    {
        var depots = await db.Depots
            .Include(d => d.Address)
            .Include(d => d.OperatingHours)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return depots.Select(MapToDto).ToList();
    }

    private static DepotDto MapToDto(Depot depot) => new(
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
