using HotChocolate;
using HotChocolate.Authorization;
using LastMile.TMS.Api.GraphQL.Inputs;
using LastMile.TMS.Application.Parcels.Commands;
using LastMile.TMS.Application.Parcels.DTOs;
using MediatR;

namespace LastMile.TMS.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class ParcelMutation
{
    [Authorize(Roles = new[] { "OperationsManager", "Admin", "Dispatcher" })]
    public async Task<ParcelDto> RegisterParcel(
        RegisterParcelInput input,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(
            new RegisterParcelCommand(
                input.ShipperAddressId,
                input.RecipientStreet1,
                input.RecipientStreet2,
                input.RecipientCity,
                input.RecipientState,
                input.RecipientPostalCode,
                input.RecipientCountryCode,
                input.RecipientIsResidential,
                input.RecipientContactName,
                input.RecipientCompanyName,
                input.RecipientPhone,
                input.RecipientEmail,
                input.Description,
                input.ServiceType,
                input.Weight,
                input.WeightUnit,
                input.Length,
                input.Width,
                input.Height,
                input.DimensionUnit,
                input.DeclaredValue,
                input.Currency,
                input.EstimatedDeliveryDate,
                input.ParcelType),
            cancellationToken);
    }
}
