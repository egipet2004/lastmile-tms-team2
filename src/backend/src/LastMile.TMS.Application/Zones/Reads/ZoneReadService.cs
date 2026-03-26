using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Zones.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Zones.Reads;

public sealed class ZoneReadService(IAppDbContext dbContext) : IZoneReadService
{
    public async Task<IReadOnlyList<ZoneDto>> GetZonesAsync(
        CancellationToken cancellationToken = default)
    {
        var zones = await dbContext.Zones
            .Include(z => z.Depot)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return zones.Select(MapToDto).ToList();
    }

    public async Task<ZoneDto?> GetZoneByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var zone = await dbContext.Zones
            .Include(z => z.Depot)
            .AsNoTracking()
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        return zone is null ? null : MapToDto(zone);
    }

    private static ZoneDto MapToDto(Domain.Entities.Zone zone) => new(
        zone.Id,
        zone.Name,
        zone.Boundary.AsText(),
        zone.IsActive,
        zone.DepotId,
        zone.Depot?.Name,
        zone.CreatedAt,
        zone.LastModifiedAt);
}
