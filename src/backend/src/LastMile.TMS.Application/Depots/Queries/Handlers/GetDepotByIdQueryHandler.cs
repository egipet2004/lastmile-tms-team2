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

        return new DepotDto
        {
            Id = depot.Id,
            Name = depot.Name,
            Address = depot.Address is not null
                ? new AddressDto
                {
                    Street1 = depot.Address.Street1,
                    Street2 = depot.Address.Street2,
                    City = depot.Address.City,
                    State = depot.Address.State,
                    PostalCode = depot.Address.PostalCode,
                    CountryCode = depot.Address.CountryCode,
                    IsResidential = depot.Address.IsResidential,
                    ContactName = depot.Address.ContactName,
                    CompanyName = depot.Address.CompanyName,
                    Phone = depot.Address.Phone,
                    Email = depot.Address.Email
                }
                : null,
            OperatingHours = depot.OperatingHours.Select(h => new OperatingHoursDto
            {
                DayOfWeek = h.DayOfWeek,
                OpenTime = h.OpenTime,
                ClosedTime = h.ClosedTime,
                IsClosed = h.IsClosed
            }).ToList(),
            IsActive = depot.IsActive,
            CreatedAt = depot.CreatedAt,
            UpdatedAt = depot.LastModifiedAt
        };
    }
}
