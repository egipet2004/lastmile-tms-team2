using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Parcels.Services;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace LastMile.TMS.Infrastructure.Services;

/// <summary>
/// Finds the first active zone whose boundary contains the given point.
/// Uses the Npgsql NetTopologySuite provider to translate Boundary.Covers(point)
/// to PostGIS ST_Covers so the GIST spatial index on Zone.Boundary is used.
/// </summary>
public class ZoneMatchingService : IZoneMatchingService
{
    private readonly IAppDbContext _db;

    public ZoneMatchingService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid?> FindZoneIdAsync(Point point, CancellationToken cancellationToken = default)
    {
        if (point is null)
            return null;

        var zoneId = await _db.Zones
            .AsNoTracking()
            .Where(z => z.IsActive && z.Boundary.Covers(point))
            .Select(z => (Guid?)z.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return zoneId;
    }
}
