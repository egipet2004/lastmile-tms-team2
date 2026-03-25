using HotChocolate.Types.Descriptors;
using LastMile.TMS.Api.GraphQL.Inputs;

namespace LastMile.TMS.Api.GraphQL.Inputs;

public class RegisterParcelInputType : InputObjectType<RegisterParcelInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<RegisterParcelInput> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}
