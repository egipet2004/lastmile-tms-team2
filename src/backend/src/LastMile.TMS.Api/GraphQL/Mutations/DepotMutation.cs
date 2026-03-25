using HotChocolate;
using HotChocolate.Authorization;
using LastMile.TMS.Api.GraphQL.Inputs;
using LastMile.TMS.Application.Depots.Commands;
using LastMile.TMS.Application.Depots.DTOs;
using MediatR;

namespace LastMile.TMS.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class DepotMutation
{
    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<DepotDto> CreateDepot(
        CreateDepotInput input,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var address = new AddressDto(
            input.Address.Street1,
            input.Address.Street2,
            input.Address.City,
            input.Address.State,
            input.Address.PostalCode,
            input.Address.CountryCode,
            input.Address.IsResidential,
            input.Address.ContactName,
            input.Address.CompanyName,
            input.Address.Phone,
            input.Address.Email);

        List<OperatingHoursDto>? operatingHours = null;
        if (input.OperatingHours is not null)
        {
            operatingHours = input.OperatingHours.Select(h => new OperatingHoursDto(
                h.DayOfWeek,
                ParseTime(h.OpenTime),
                ParseTime(h.ClosedTime),
                h.IsClosed)).ToList();
        }

        return await mediator.Send(
            new CreateDepotCommand(input.Name, address, operatingHours, input.IsActive),
            cancellationToken);
    }

    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<DepotDto?> UpdateDepot(
        Guid id,
        UpdateDepotInput input,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        AddressDto? address = null;
        if (input.Address is not null)
        {
            address = new AddressDto(
                input.Address.Street1,
                input.Address.Street2,
                input.Address.City,
                input.Address.State,
                input.Address.PostalCode,
                input.Address.CountryCode,
                input.Address.IsResidential,
                input.Address.ContactName,
                input.Address.CompanyName,
                input.Address.Phone,
                input.Address.Email);
        }

        List<OperatingHoursDto>? operatingHours = null;
        if (input.OperatingHours is not null)
        {
            operatingHours = input.OperatingHours.Select(h => new OperatingHoursDto(
                h.DayOfWeek,
                ParseTime(h.OpenTime),
                ParseTime(h.ClosedTime),
                h.IsClosed)).ToList();
        }

        return await mediator.Send(
            new UpdateDepotCommand(id, input.Name, address, operatingHours, input.IsActive),
            cancellationToken);
    }

    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<bool> DeleteDepot(
        Guid id,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new DeleteDepotCommand(id), cancellationToken);
    }

    private static TimeOnly? ParseTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return TimeOnly.Parse(value);
    }
}
