using LastMile.TMS.Application.Depots.DTOs;
using MediatR;

namespace LastMile.TMS.Application.Depots.Queries;

public record GetAllDepotsQuery : IRequest<List<DepotDto>>;
