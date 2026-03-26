using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Depots.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Depots.Reads;

public sealed class DepotReadService(IAppDbContext dbContext) : IDepotReadService
{
    public IQueryable<DepotDto> GetDepots() =>
        dbContext.Depots
            .AsNoTracking()
            .Select(MapToDto());

    public IQueryable<DepotDto> GetDepotById(Guid id) =>
        dbContext.Depots
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(MapToDto());

    private static System.Linq.Expressions.Expression<Func<Domain.Entities.Depot, DepotDto>> MapToDto() =>
        d => new DepotDto(
            d.Id,
            d.Name,
            d.Address != null
                ? new AddressDto(
                    d.Address.Street1,
                    d.Address.Street2,
                    d.Address.City,
                    d.Address.State,
                    d.Address.PostalCode,
                    d.Address.CountryCode,
                    d.Address.IsResidential,
                    d.Address.ContactName,
                    d.Address.CompanyName,
                    d.Address.Phone,
                    d.Address.Email)
                : null,
            d.OperatingHours.Select(h => new OperatingHoursDto(
                h.DayOfWeek, h.OpenTime, h.ClosedTime, h.IsClosed)).ToList(),
            d.IsActive,
            d.CreatedAt,
            d.LastModifiedAt);
}
