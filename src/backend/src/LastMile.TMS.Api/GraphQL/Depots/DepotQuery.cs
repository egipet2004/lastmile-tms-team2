using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using LastMile.TMS.Application.Depots.DTOs;
using LastMile.TMS.Application.Depots.Queries;
using LastMile.TMS.Application.Depots.Reads;
using MediatR;

namespace LastMile.TMS.Api.GraphQL.Depots;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class DepotQuery
{
    [Authorize(Roles = new[] { "OperationsManager", "Admin", "Dispatcher" })]
    [UseSorting]
    [UseFiltering]
    public IQueryable<DepotDto> GetDepots(
        [Service] IDepotReadService readService = null!) =>
        readService.GetDepots();

    [Authorize(Roles = new[] { "OperationsManager", "Admin", "Dispatcher" })]
    public Task<DepotDto?> GetDepot(
        Guid id,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default) =>
        mediator.Send(new GetDepotByIdQuery(id), cancellationToken);
}
