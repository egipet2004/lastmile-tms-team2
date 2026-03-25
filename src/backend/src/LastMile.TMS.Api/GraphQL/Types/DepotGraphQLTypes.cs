using LastMile.TMS.Application.Depots.DTOs;

namespace LastMile.TMS.Api.GraphQL.Types;

public class AddressType : ObjectType<AddressDto>
{
    protected override void Configure(IObjectTypeDescriptor<AddressDto> descriptor)
    {
        descriptor.Name("Address");
        descriptor.BindFieldsImplicitly();
    }
}

public class OperatingHoursType : ObjectType<OperatingHoursDto>
{
    protected override void Configure(IObjectTypeDescriptor<OperatingHoursDto> descriptor)
    {
        descriptor.Name("OperatingHours");
        descriptor.BindFieldsImplicitly();
    }
}

public class DepotType : ObjectType<DepotDto>
{
    protected override void Configure(IObjectTypeDescriptor<DepotDto> descriptor)
    {
        descriptor.Name("Depot");
        descriptor.BindFieldsImplicitly();
    }
}
