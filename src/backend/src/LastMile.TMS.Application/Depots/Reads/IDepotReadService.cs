using LastMile.TMS.Application.Depots.DTOs;

namespace LastMile.TMS.Application.Depots.Reads;

public interface IDepotReadService
{
    IQueryable<DepotDto> GetDepots();
    IQueryable<DepotDto> GetDepotById(Guid id);
}
