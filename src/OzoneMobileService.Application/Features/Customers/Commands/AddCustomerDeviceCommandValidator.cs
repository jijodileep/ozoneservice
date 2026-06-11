using FluentValidation;

namespace OzoneMobileService.Application.Features.Customers.Commands;

public sealed class AddCustomerDeviceCommandValidator : AbstractValidator<AddCustomerDeviceCommand>
{
    public AddCustomerDeviceCommandValidator()
    {
        RuleFor(x => x.VariantId).NotEmpty();
        RuleFor(x => x.Imei).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Imei));
    }
}
