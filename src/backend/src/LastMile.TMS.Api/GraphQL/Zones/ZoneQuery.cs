using HotChocolate;
using LastMile.TMS.Application.Zones.DTOs;
using LastMile.TMS.Application.Zones.Reads;

namespace LastMile.TMS.Api.GraphQL.Zones;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class ZoneQuery
{
    public Task<IReadOnlyList<ZoneDto>> GetZones(
        [Service] IZoneReadService readService = null!,
        CancellationToken cancellationToken = default) =>
        readService.GetZonesAsync(cancellationToken);

    public Task<ZoneDto?> GetZone(
        Guid id,
        [Service] IZoneReadService readService = null!,
        CancellationToken cancellationToken = default) =>
        readService.GetZoneByIdAsync(id, cancellationToken);
}
