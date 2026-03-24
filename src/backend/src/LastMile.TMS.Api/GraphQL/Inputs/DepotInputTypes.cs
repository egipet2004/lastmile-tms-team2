namespace LastMile.TMS.Api.GraphQL.Inputs;

public class AddressInputType : InputObjectType<AddressInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<AddressInput> descriptor)
    {
        descriptor.Name("AddressInput");
        descriptor.BindFieldsImplicitly();
    }
}

public class OperatingHoursInputType : InputObjectType<OperatingHoursInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<OperatingHoursInput> descriptor)
    {
        descriptor.Name("OperatingHoursInput");
        descriptor.BindFieldsImplicitly();
    }
}

public class CreateDepotInputType : InputObjectType<CreateDepotInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CreateDepotInput> descriptor)
    {
        descriptor.Name("CreateDepotInput");
        descriptor.BindFieldsImplicitly();
    }
}

public class UpdateDepotInputType : InputObjectType<UpdateDepotInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<UpdateDepotInput> descriptor)
    {
        descriptor.Name("UpdateDepotInput");
        descriptor.BindFieldsImplicitly();
    }
}
