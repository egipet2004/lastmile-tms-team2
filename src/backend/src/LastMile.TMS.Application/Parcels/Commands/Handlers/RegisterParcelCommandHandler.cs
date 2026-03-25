using LastMile.TMS.Application.Common.Interfaces;
using LastMile.TMS.Application.Parcels.Commands;
using LastMile.TMS.Application.Parcels.DTOs;
using LastMile.TMS.Application.Parcels.Services;
using LastMile.TMS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LastMile.TMS.Application.Parcels.Commands.Handlers;

public class RegisterParcelCommandHandler(
    IAppDbContext db,
    IGeocodingService geocodingService,
    IZoneMatchingService zoneMatchingService)
    : IRequestHandler<RegisterParcelCommand, ParcelDto>
{
    public async Task<ParcelDto> Handle(RegisterParcelCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate shipper address exists
        var shipperExists = await db.Addresses
            .AnyAsync(a => a.Id == request.ShipperAddressId, cancellationToken);
        if (!shipperExists)
            throw new ArgumentException($"Shipper address with ID '{request.ShipperAddressId}' was not found.");

        // 2. Build recipient address string and geocode
        var addressString = BuildAddressString(request);
        var point = await geocodingService.GeocodeAsync(addressString, cancellationToken);

        if (point is null)
        {
            throw new InvalidOperationException(
                $"Could not determine zone: recipient address could not be geocoded. Address: {addressString}");
        }

        // 3. Find matching zone via point-in-polygon
        var zoneId = await zoneMatchingService.FindZoneIdAsync(point, cancellationToken);

        if (zoneId is null)
        {
            throw new InvalidOperationException(
                $"No active zone covers the recipient address location. Address: {addressString}");
        }

        // 4. Load zone + depot (single query)
        var zone = await db.Zones
            .Include(z => z.Depot)
            .FirstOrDefaultAsync(z => z.Id == zoneId, cancellationToken);

        if (zone is null)
            throw new InvalidOperationException($"Zone with ID '{zoneId}' was found but could not be loaded.");

        // 5. Create recipient address and parcel — persisted together
        var recipientAddress = new Address
        {
            Street1 = request.RecipientStreet1,
            Street2 = request.RecipientStreet2,
            City = request.RecipientCity,
            State = request.RecipientState,
            PostalCode = request.RecipientPostalCode,
            CountryCode = request.RecipientCountryCode,
            IsResidential = request.RecipientIsResidential,
            ContactName = request.RecipientContactName,
            CompanyName = request.RecipientCompanyName,
            Phone = request.RecipientPhone,
            Email = request.RecipientEmail,
            GeoLocation = point,
        };

        var parcel = new Parcel
        {
            TrackingNumber = Parcel.GenerateTrackingNumber(),
            Description = request.Description,
            ServiceType = request.ServiceType,
            Status = Domain.Enums.ParcelStatus.Registered,
            ShipperAddressId = request.ShipperAddressId,
            RecipientAddress = recipientAddress,
            Weight = request.Weight,
            WeightUnit = request.WeightUnit,
            Length = request.Length,
            Width = request.Width,
            Height = request.Height,
            DimensionUnit = request.DimensionUnit,
            DeclaredValue = request.DeclaredValue,
            Currency = request.Currency,
            EstimatedDeliveryDate = request.EstimatedDeliveryDate,
            ParcelType = request.ParcelType,
            ZoneId = zoneId.Value,
        };

        db.Parcels.Add(parcel);
        await db.SaveChangesAsync(cancellationToken);

        return new ParcelDto(
            parcel.Id,
            parcel.TrackingNumber,
            parcel.Description,
            parcel.ServiceType.ToString(),
            parcel.Status.ToString(),
            parcel.Weight,
            parcel.WeightUnit.ToString(),
            parcel.Length,
            parcel.Width,
            parcel.Height,
            parcel.DimensionUnit.ToString(),
            parcel.DeclaredValue,
            parcel.Currency,
            parcel.EstimatedDeliveryDate,
            parcel.DeliveryAttempts,
            parcel.ParcelType,
            zoneId.Value,
            zone.Name,
            zone.DepotId,
            zone.Depot?.Name,
            parcel.CreatedAt,
            parcel.LastModifiedAt);
    }

    private static string BuildAddressString(RegisterParcelCommand r)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(r.RecipientStreet1)) parts.Add(r.RecipientStreet1);
        if (!string.IsNullOrWhiteSpace(r.RecipientStreet2)) parts.Add(r.RecipientStreet2);
        if (!string.IsNullOrWhiteSpace(r.RecipientCity)) parts.Add(r.RecipientCity);
        if (!string.IsNullOrWhiteSpace(r.RecipientState)) parts.Add(r.RecipientState);
        if (!string.IsNullOrWhiteSpace(r.RecipientPostalCode)) parts.Add(r.RecipientPostalCode);
        if (!string.IsNullOrWhiteSpace(r.RecipientCountryCode)) parts.Add(r.RecipientCountryCode);
        return string.Join(", ", parts);
    }
}
