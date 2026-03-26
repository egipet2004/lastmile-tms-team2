using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Drivers.Reads;

public sealed class DriverReadService(IAppDbContext dbContext) : IDriverReadService
{
    public IQueryable<DriverReadModel> GetDrivers(Guid? depotId = null)
    {
        var query = dbContext.Drivers
            .AsNoTracking()
            .Where(d => d.Status == DriverStatus.Active);

        if (depotId.HasValue)
        {
            query = query.Where(d => d.DepotId == depotId.Value);
        }

        return query
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .Select(d => new DriverReadModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                DisplayName = d.FirstName + " " + d.LastName,
                Phone = d.Phone,
                Email = d.Email,
                DepotId = d.DepotId,
                Status = d.Status,
            });
    }
}
