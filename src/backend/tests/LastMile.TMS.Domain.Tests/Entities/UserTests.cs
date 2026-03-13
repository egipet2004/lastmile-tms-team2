using FluentAssertions;
using LastMile.TMS.Domain.Entities;

namespace LastMile.TMS.Domain.Tests.Entities;

public class UserTests
{

    [Fact]
    public void User_ShouldExtendBaseAuditableEntity()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.CreatedAt.Should().Be(default);
        user.CreatedBy.Should().BeNull();
    }

    [Fact]
    public void User_ShouldHaveRequiredProperties()
    {
        // Arrange & Act
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890"
        };

        // Assert
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john.doe@example.com");
        user.PhoneNumber.Should().Be("+1234567890");
    }

    [Fact]
    public void User_ShouldHaveIsActiveProperty()
    {
        // Arrange & Act
        var user = new User { IsActive = true };

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_ShouldHaveIsActiveTrueByDefault()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_ShouldHaveRoleId()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var user = new User { RoleId = roleId };

        // Assert
        user.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void User_ShouldBeSoftDeletable()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act - soft delete
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }
}