using NetTopologySuite.Geometries;

namespace LastMile.TMS.Application.Parcels.Services;

/// <summary>
/// Resolves a textual address to a geographic point (SRID 4326).
/// Returns null when the address cannot be resolved.
/// </summary>
public interface IGeocodingService
{
    Task<Point?> GeocodeAsync(string address, CancellationToken cancellationToken = default);
}
