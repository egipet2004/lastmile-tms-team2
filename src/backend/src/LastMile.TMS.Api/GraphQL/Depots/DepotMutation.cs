using HotChocolate;
using HotChocolate.Authorization;
using LastMile.TMS.Application.Depots.Commands;
using LastMile.TMS.Application.Depots.DTOs;
using MediatR;

namespace LastMile.TMS.Api.GraphQL.Depots;

[ExtendObjectType(OperationTypeNames.Mutation)]
public sealed class DepotMutation
{
    [Authorize(Roles = new[] { "OperationsManager", "Admin" })]
    public async Task<DepotDto> CreateDepot(
        CreateDepotInput input,
        [Service] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var address = new AddressDto
        {
            Street1 = input.Address.Street1,
            Street2 = input.Address.Street2,
            City = input.Address.City,
            State = input.Address.State,
            PostalCode = input.Address.PostalCode,
            CountryCode = input.Address.CountryCode,
            IsResidential = input.Address.IsResidential,
            ContactName = input.Address.ContactName,
            CompanyName = input.Address.CompanyName,
            Phone = input.Address.Phone,
            Email = input.Address.Email
        };

        List<OperatingHoursDto>? operatingHours = null;
        if (input.OperatingHours is not null)
        {
            operatingHours = input.OperatingHours.Select(h => new OperatingHoursDto
            {
                DayOfWeek = h.DayOfWeek,
                OpenTime = ParseTime(h.OpenTime),
                ClosedTime = ParseTime(h.ClosedTime),
                IsClosed = h.IsClosed
            }).ToList();
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
            address = new AddressDto
            {
                Street1 = input.Address.Street1,
                Street2 = input.Address.Street2,
                City = input.Address.City,
                State = input.Address.State,
                PostalCode = input.Address.PostalCode,
                CountryCode = input.Address.CountryCode,
                IsResidential = input.Address.IsResidential,
                ContactName = input.Address.ContactName,
                CompanyName = input.Address.CompanyName,
                Phone = input.Address.Phone,
                Email = input.Address.Email
            };
        }

        List<OperatingHoursDto>? operatingHours = null;
        if (input.OperatingHours is not null)
        {
            operatingHours = input.OperatingHours.Select(h => new OperatingHoursDto
            {
                DayOfWeek = h.DayOfWeek,
                OpenTime = ParseTime(h.OpenTime),
                ClosedTime = ParseTime(h.ClosedTime),
                IsClosed = h.IsClosed
            }).ToList();
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
