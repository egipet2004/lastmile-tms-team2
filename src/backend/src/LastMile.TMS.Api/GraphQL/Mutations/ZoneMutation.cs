using HotChocolate;
using HotChocolate.Authorization;
using LastMile.TMS.Api.GraphQL.Inputs;
using LastMile.TMS.Application.Zones.Commands;
using LastMile.TMS.Application.Zones.DTOs;
using MediatR;

namespace LastMile.TMS.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class ZoneMutation
{
    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<ZoneDto> CreateZone(
        CreateZoneInput input,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(
            new CreateZoneCommand(
                input.Name,
                input.DepotId,
                input.IsActive,
                input.GeoJson,
                input.Coordinates,
                input.BoundaryWkt),
            cancellationToken);
    }

    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<ZoneDto?> UpdateZone(
        Guid id,
        UpdateZoneInput input,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(
            new UpdateZoneCommand(
                id,
                input.Name,
                input.DepotId,
                input.IsActive,
                input.GeoJson,
                input.Coordinates,
                input.BoundaryWkt),
            cancellationToken);
    }

    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<bool> DeleteZone(
        Guid id,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new DeleteZoneCommand(id), cancellationToken);
    }
}
