using LastMile.TMS.Domain.Enums;

namespace LastMile.TMS.Api.GraphQL.Inputs;

public class RegisterParcelInput
{
    // Shipper — required
    public Guid ShipperAddressId { get; set; }

    // Recipient address — required, used for zone auto-assignment
    public string RecipientStreet1 { get; set; } = null!;
    public string? RecipientStreet2 { get; set; }
    public string RecipientCity { get; set; } = null!;
    public string RecipientState { get; set; } = null!;
    public string RecipientPostalCode { get; set; } = null!;
    public string RecipientCountryCode { get; set; } = null!;
    public bool RecipientIsResidential { get; set; } = true;
    public string? RecipientContactName { get; set; }
    public string? RecipientCompanyName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientEmail { get; set; }

    // Parcel details
    public string? Description { get; set; }
    public ServiceType ServiceType { get; set; } = ServiceType.Standard;
    public decimal Weight { get; set; }
    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kg;
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public DimensionUnit DimensionUnit { get; set; } = DimensionUnit.Cm;
    public decimal DeclaredValue { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTimeOffset EstimatedDeliveryDate { get; set; }
    public string? ParcelType { get; set; }
}
