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
        d => new DepotDto
        {
            Id = d.Id,
            Name = d.Name,
            Address = d.Address != null
                ? new AddressDto
                {
                    Street1 = d.Address.Street1,
                    Street2 = d.Address.Street2,
                    City = d.Address.City,
                    State = d.Address.State,
                    PostalCode = d.Address.PostalCode,
                    CountryCode = d.Address.CountryCode,
                    IsResidential = d.Address.IsResidential,
                    ContactName = d.Address.ContactName,
                    CompanyName = d.Address.CompanyName,
                    Phone = d.Address.Phone,
                    Email = d.Address.Email
                }
                : null,
            OperatingHours = d.OperatingHours.Select(h => new OperatingHoursDto
            {
                DayOfWeek = h.DayOfWeek,
                OpenTime = h.OpenTime,
                ClosedTime = h.ClosedTime,
                IsClosed = h.IsClosed
            }).ToList(),
            IsActive = d.IsActive,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.LastModifiedAt
        };
}
