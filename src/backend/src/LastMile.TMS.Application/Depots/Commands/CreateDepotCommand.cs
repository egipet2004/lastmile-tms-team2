using LastMile.TMS.Application.Depots.DTOs;
using MediatR;

namespace LastMile.TMS.Application.Depots.Commands;

public record CreateDepotCommand(
    string Name,
    AddressDto Address,
    List<OperatingHoursDto>? OperatingHours,
    bool IsActive = true
) : IRequest<DepotDto>;
