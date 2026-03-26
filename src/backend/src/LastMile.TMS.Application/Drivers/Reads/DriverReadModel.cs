using LastMile.TMS.Domain.Enums;

namespace LastMile.TMS.Application.Drivers.Reads;

public class DriverReadModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public Guid DepotId { get; set; }
    public DriverStatus Status { get; set; }
}
