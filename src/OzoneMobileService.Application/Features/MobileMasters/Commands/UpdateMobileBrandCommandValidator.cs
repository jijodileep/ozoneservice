using FluentValidation;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed class UpdateMobileBrandCommandValidator : AbstractValidator<UpdateMobileBrandCommand>
{
    public UpdateMobileBrandCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
