using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using LastMile.TMS.Application.Drivers.Reads;

namespace LastMile.TMS.Api.GraphQL.Drivers;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class DriverQuery
{
    [Authorize(Roles = new[] { "OperationsManager", "Admin", "Dispatcher" })]
    [UseProjection]
    public IQueryable<DriverReadModel> GetDrivers(
        Guid? depotId = null,
        [Service] IDriverReadService readService = null!) =>
        readService.GetDrivers(depotId);
}
