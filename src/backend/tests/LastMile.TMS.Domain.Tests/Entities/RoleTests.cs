using FluentAssertions;
using LastMile.TMS.Domain.Entities;

namespace LastMile.TMS.Domain.Tests.Entities;

public class RoleTests
{

    [Fact]
    public void Role_ShouldHavePredefinedRoles()
    {
        // Assert - All predefined roles should exist
        Enum.GetValues<PredefinedRole>().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(PredefinedRole.Admin)]
    [InlineData(PredefinedRole.OperationsManager)]
    [InlineData(PredefinedRole.Dispatcher)]
    [InlineData(PredefinedRole.WarehouseOperator)]
    [InlineData(PredefinedRole.Driver)]
    public void PredefinedRole_ShouldContainExpectedRoles(PredefinedRole role)
    {
        // Assert
        role.Should().BeDefined();
    }

    [Fact]
    public void Role_ShouldSetNameFromPredefinedRole()
    {
        // Arrange & Act
        var role = new Role
        {
            Name = PredefinedRole.Admin.ToString(),
            Description = "System administrator with full access"
        };

        // Assert
        role.Name.Should().Be("Admin");
    }

    [Fact]
    public void Role_ShouldAllowIsDefaultFlag()
    {
        // Arrange & Act
        var role = new Role
        {
            Name = PredefinedRole.Driver.ToString(),
            IsDefault = true
        };

        // Assert
        role.IsDefault.Should().BeTrue();
    }
}