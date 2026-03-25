using FluentValidation;
using LastMile.TMS.Application.Depots.Commands;

namespace LastMile.TMS.Application.Depots.Validators;

public class CreateDepotCommandValidator : AbstractValidator<CreateDepotCommand>
{
    public CreateDepotCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Depot name is required.")
            .MaximumLength(200).WithMessage("Depot name must not exceed 200 characters.");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required.");

        RuleFor(x => x.Address.Street1)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(200);

        RuleFor(x => x.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100);

        RuleFor(x => x.Address.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(100);

        RuleFor(x => x.Address.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .MaximumLength(20);

        RuleFor(x => x.Address.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2, 3).WithMessage("Country code must be 2 or 3 characters.");

        When(x => x.OperatingHours is not null, () =>
        {
            RuleFor(x => x.OperatingHours!)
                .Must(hours => hours.Count <= 7)
                .WithMessage("Cannot have more than 7 operating hours entries (one per day).");

            RuleForEach(x => x.OperatingHours!)
                .ChildRules(hours =>
                {
                    hours.RuleFor(h => h.OpenTime)
                        .NotNull().WithMessage("Open time is required when not closed.")
                        .When(h => !h.IsClosed);
                    hours.RuleFor(h => h.ClosedTime)
                        .NotNull().WithMessage("Closed time is required when not closed.")
                        .When(h => !h.IsClosed);
                    hours.RuleFor(h => h.ClosedTime)
                        .GreaterThan(h => h.OpenTime)
                        .WithMessage("Closed time must be after open time.")
                        .When(h => !h.IsClosed && h.OpenTime is not null && h.ClosedTime is not null);
                });
        });
    }
}
