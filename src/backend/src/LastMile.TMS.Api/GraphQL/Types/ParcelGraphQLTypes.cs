using HotChocolate.Types.Descriptors;
using LastMile.TMS.Application.Parcels.DTOs;

namespace LastMile.TMS.Api.GraphQL.Types;

public class ParcelType : ObjectType<ParcelDto>
{
    protected override void Configure(IObjectTypeDescriptor<ParcelDto> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}
