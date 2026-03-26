using LastMile.TMS.Application.Zones.DTOs;

namespace LastMile.TMS.Application.Zones.Reads;

public interface IZoneReadService
{
    Task<IReadOnlyList<ZoneDto>> GetZonesAsync(CancellationToken cancellationToken = default);
    Task<ZoneDto?> GetZoneByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
