namespace LastMile.TMS.Application.Zones.DTOs;

public sealed record ZoneDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Boundary { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public Guid DepotId { get; init; }
    public string? DepotName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public ZoneDto() { }
}
