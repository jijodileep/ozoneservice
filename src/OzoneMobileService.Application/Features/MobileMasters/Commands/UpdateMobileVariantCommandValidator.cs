using FluentValidation;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed class UpdateMobileVariantCommandValidator : AbstractValidator<UpdateMobileVariantCommand>
{
    public UpdateMobileVariantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
