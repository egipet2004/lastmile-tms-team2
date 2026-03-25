using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Depots.Commands;
using LastMile.TMS.Application.Depots.DTOs;
using LastMile.TMS.Domain.Entities;
using MediatR;

namespace LastMile.TMS.Application.Depots.Commands.Handlers;

public class CreateDepotCommandHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateDepotCommand, DepotDto>
{
    public async Task<DepotDto> Handle(CreateDepotCommand request, CancellationToken cancellationToken)
    {
        var address = new Address
        {
            Street1 = request.Address.Street1,
            Street2 = request.Address.Street2,
            City = request.Address.City,
            State = request.Address.State,
            PostalCode = request.Address.PostalCode,
            CountryCode = request.Address.CountryCode.ToUpperInvariant(),
            IsResidential = request.Address.IsResidential,
            ContactName = request.Address.ContactName,
            CompanyName = request.Address.CompanyName,
            Phone = request.Address.Phone,
            Email = request.Address.Email,
        };

        var depot = new Depot
        {
            Name = request.Name,
            Address = address,
            IsActive = request.IsActive,
        };

        if (request.OperatingHours is not null)
        {
            foreach (var hours in request.OperatingHours)
            {
                depot.OperatingHours.Add(new OperatingHours
                {
                    DayOfWeek = hours.DayOfWeek,
                    OpenTime = hours.OpenTime,
                    ClosedTime = hours.ClosedTime,
                    IsClosed = hours.IsClosed,
                });
            }
        }

        db.Depots.Add(depot);
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
