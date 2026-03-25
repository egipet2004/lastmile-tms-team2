using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Depots.Commands;
using LastMile.TMS.Application.Depots.DTOs;
using LastMile.TMS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Depots.Commands.Handlers;

public class UpdateDepotCommandHandler(
    IAppDbContext db)
    : IRequestHandler<UpdateDepotCommand, DepotDto?>
{
    public async Task<DepotDto?> Handle(UpdateDepotCommand request, CancellationToken cancellationToken)
    {
        var depot = await db.Depots
            .Include(d => d.Address)
            .Include(d => d.OperatingHours)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (depot is null)
            return null;

        depot.Name = request.Name;
        depot.IsActive = request.IsActive;

        if (request.Address is not null)
        {
            depot.Address.Street1 = request.Address.Street1;
            depot.Address.Street2 = request.Address.Street2;
            depot.Address.City = request.Address.City;
            depot.Address.State = request.Address.State;
            depot.Address.PostalCode = request.Address.PostalCode;
            depot.Address.CountryCode = request.Address.CountryCode.ToUpperInvariant();
            depot.Address.IsResidential = request.Address.IsResidential;
            depot.Address.ContactName = request.Address.ContactName;
            depot.Address.CompanyName = request.Address.CompanyName;
            depot.Address.Phone = request.Address.Phone;
            depot.Address.Email = request.Address.Email;
        }

        if (request.OperatingHours is not null)
        {
            depot.OperatingHours.Clear();
            foreach (var hours in request.OperatingHours)
            {
                depot.OperatingHours.Add(new OperatingHours
                {
                    DepotId = depot.Id,
                    DayOfWeek = hours.DayOfWeek,
                    OpenTime = hours.OpenTime,
                    ClosedTime = hours.ClosedTime,
                    IsClosed = hours.IsClosed,
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return MapToDto(depot);
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
