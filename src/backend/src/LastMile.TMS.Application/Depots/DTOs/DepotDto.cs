namespace LastMile.TMS.Application.Depots.DTOs;

public record DepotDto(
    Guid Id,
    string Name,
    AddressDto? Address,
    List<OperatingHoursDto>? OperatingHours,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

public record AddressDto(
    string Street1,
    string? Street2,
    string City,
    string State,
    string PostalCode,
    string CountryCode,
    bool IsResidential = false,
    string? ContactName = null,
    string? CompanyName = null,
    string? Phone = null,
    string? Email = null
);

public record OperatingHoursDto(
    DayOfWeek DayOfWeek,
    TimeOnly? OpenTime,
    TimeOnly? ClosedTime,
    bool IsClosed
);
