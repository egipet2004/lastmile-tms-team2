namespace LastMile.TMS.Api.GraphQL.Inputs;

public class AddressInput
{
    public string Street1 { get; set; } = null!;
    public string? Street2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public bool IsResidential { get; set; }
    public string? ContactName { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class OperatingHoursInput
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? ClosedTime { get; set; }
    public bool IsClosed { get; set; }
}

public class CreateDepotInput
{
    public string Name { get; set; } = null!;
    public AddressInput Address { get; set; } = null!;
    public List<OperatingHoursInput>? OperatingHours { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateDepotInput
{
    public string Name { get; set; } = null!;
    public AddressInput? Address { get; set; }
    public List<OperatingHoursInput>? OperatingHours { get; set; }
    public bool IsActive { get; set; }
}
