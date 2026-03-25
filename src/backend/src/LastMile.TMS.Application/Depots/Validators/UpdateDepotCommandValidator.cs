using FluentValidation;
using LastMile.TMS.Application.Depots.Commands;

namespace LastMile.TMS.Application.Depots.Validators;

public class UpdateDepotCommandValidator : AbstractValidator<UpdateDepotCommand>
{
    public UpdateDepotCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Depot ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Depot name is required.")
            .MaximumLength(200).WithMessage("Depot name must not exceed 200 characters.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.Street1)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(200);

            RuleFor(x => x.Address!.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.Address!.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100);

            RuleFor(x => x.Address!.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .MaximumLength(20);

            RuleFor(x => x.Address!.CountryCode)
                .NotEmpty().WithMessage("Country code is required.")
                .Length(2, 3).WithMessage("Country code must be 2 or 3 characters.");
        });
    }
}
