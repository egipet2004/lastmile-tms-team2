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

    private static DepotDto MapToDto(Depot depot) => new()
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
