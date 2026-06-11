using FluentValidation;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Application.Features.Customers.Commands;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .Must(m => PhoneNormalizer.TryNormalize(m, out _))
            .WithMessage("Enter a valid 10-digit mobile number.");
        RuleFor(x => x.Email).MaximumLength(256).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Address));
    }
}
