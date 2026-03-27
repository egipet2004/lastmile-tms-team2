namespace LastMile.TMS.Application.Depots.DTOs;

public sealed record DepotDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public AddressDto? Address { get; init; }
    public List<OperatingHoursDto>? OperatingHours { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public DepotDto() { }
}

public sealed record AddressDto
{
    public string Street1 { get; init; } = string.Empty;
    public string? Street2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
    public bool IsResidential { get; init; }
    public string? ContactName { get; init; }
    public string? CompanyName { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }

    public AddressDto() { }
}

public sealed record OperatingHoursDto
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly? OpenTime { get; init; }
    public TimeOnly? ClosedTime { get; init; }
    public bool IsClosed { get; init; }

    public OperatingHoursDto() { }
}
