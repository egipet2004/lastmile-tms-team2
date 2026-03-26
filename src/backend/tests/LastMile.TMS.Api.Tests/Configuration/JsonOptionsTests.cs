using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using LastMile.TMS.Api.Configuration;
using LastMile.TMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LastMile.TMS.Api.Tests.Configuration;

public class JsonOptionsTests
{
    [Fact]
    public void ApiJsonOptions_SerializeEnum_AsString()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        services.AddLastMileApi(new ConfigurationBuilder().Build());

        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<JsonOptions>>().Value;

        var json = JsonSerializer.Serialize(VehicleStatus.Available, options.JsonSerializerOptions);

        json.Should().Be("\"Available\"", because: "enums should serialize as PascalCase strings");
    }

    [Fact]
    public void ApiJsonOptions_DeserializeEnum_FromString()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        services.AddLastMileApi(new ConfigurationBuilder().Build());

        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<JsonOptions>>().Value;

        var result = JsonSerializer.Deserialize<VehicleStatus>("\"InUse\"", options.JsonSerializerOptions);

        result.Should().Be(VehicleStatus.InUse);
    }
}
