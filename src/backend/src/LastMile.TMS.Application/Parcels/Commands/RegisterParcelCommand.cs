using LastMile.TMS.Application.Parcels.DTOs;
using LastMile.TMS.Domain.Enums;
using MediatR;

namespace LastMile.TMS.Application.Parcels.Commands;

public record RegisterParcelCommand(
    // Shipper — required
    Guid ShipperAddressId,

    // Recipient — full inline address (required for zone auto-assignment)
    string RecipientStreet1,
    string? RecipientStreet2,
    string RecipientCity,
    string RecipientState,
    string RecipientPostalCode,
    string RecipientCountryCode,
    bool RecipientIsResidential,
    string? RecipientContactName,
    string? RecipientCompanyName,
    string? RecipientPhone,
    string? RecipientEmail,

    // Parcel details
    string? Description,
    ServiceType ServiceType,
    decimal Weight,
    WeightUnit WeightUnit,
    decimal Length,
    decimal Width,
    decimal Height,
    DimensionUnit DimensionUnit,
    decimal DeclaredValue,
    string Currency,
    DateTimeOffset EstimatedDeliveryDate,
    string? ParcelType)
    : IRequest<ParcelDto>;
