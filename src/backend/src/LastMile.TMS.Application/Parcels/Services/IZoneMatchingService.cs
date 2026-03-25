using NetTopologySuite.Geometries;

namespace LastMile.TMS.Application.Parcels.Services;

/// <summary>
/// Finds the active zone whose boundary contains the given point.
/// Returns null when no zone covers the point.
/// </summary>
public interface IZoneMatchingService
{
    Task<Guid?> FindZoneIdAsync(Point point, CancellationToken cancellationToken = default);
}
