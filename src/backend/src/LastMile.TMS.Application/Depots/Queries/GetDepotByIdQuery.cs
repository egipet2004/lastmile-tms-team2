using LastMile.TMS.Application.Depots.DTOs;
using MediatR;

namespace LastMile.TMS.Application.Depots.Queries;

public record GetDepotByIdQuery(Guid Id) : IRequest<DepotDto?>;
