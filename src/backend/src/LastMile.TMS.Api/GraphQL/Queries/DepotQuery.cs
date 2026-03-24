using HotChocolate;
using LastMile.TMS.Application.Depots.DTOs;
using LastMile.TMS.Application.Depots.Queries;
using MediatR;

namespace LastMile.TMS.Api.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class DepotQuery
{
    public async Task<List<DepotDto>> GetDepots(
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetAllDepotsQuery(), cancellationToken);
    }

    public async Task<DepotDto?> GetDepot(
        Guid id,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetDepotByIdQuery(id), cancellationToken);
    }
}
