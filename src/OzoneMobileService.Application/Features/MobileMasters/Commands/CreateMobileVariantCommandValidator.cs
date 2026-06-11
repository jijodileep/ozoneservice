using FluentValidation;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed class CreateMobileVariantCommandValidator : AbstractValidator<CreateMobileVariantCommand>
{
    public CreateMobileVariantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
