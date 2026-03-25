using FluentValidation;
using LastMile.TMS.Application.Parcels.Commands;

namespace LastMile.TMS.Application.Parcels.Validators;

public class RegisterParcelCommandValidator : AbstractValidator<RegisterParcelCommand>
{
    public RegisterParcelCommandValidator()
    {
        // Shipper — required
        RuleFor(x => x.ShipperAddressId)
            .NotEmpty().WithMessage("ShipperAddressId is required.");

        // Recipient address — required fields for geocoding
        RuleFor(x => x.RecipientStreet1)
            .NotEmpty().WithMessage("Recipient street address is required.");

        RuleFor(x => x.RecipientCity)
            .NotEmpty().WithMessage("Recipient city is required.");

        RuleFor(x => x.RecipientState)
            .NotEmpty().WithMessage("Recipient state is required.");

        RuleFor(x => x.RecipientPostalCode)
            .NotEmpty().WithMessage("Recipient postal code is required.");

        RuleFor(x => x.RecipientCountryCode)
            .NotEmpty().WithMessage("Recipient country code is required.")
            .Length(2, 3).WithMessage("Country code must be 2 or 3 characters (e.g. US, UK).");

        // Parcel dimensions
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.");

        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Length must be greater than 0.");

        RuleFor(x => x.Width)
            .GreaterThan(0).WithMessage("Width must be greater than 0.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.");

        RuleFor(x => x.DeclaredValue)
            .GreaterThanOrEqualTo(0).WithMessage("Declared value cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-character ISO code (e.g. USD).");

        RuleFor(x => x.EstimatedDeliveryDate)
            .NotEmpty().WithMessage("Estimated delivery date is required.")
            .Must(d => d > DateTimeOffset.UtcNow)
            .WithMessage("Estimated delivery date must be in the future.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.ParcelType)
            .MaximumLength(50).WithMessage("Parcel type must not exceed 50 characters.");
    }
}
