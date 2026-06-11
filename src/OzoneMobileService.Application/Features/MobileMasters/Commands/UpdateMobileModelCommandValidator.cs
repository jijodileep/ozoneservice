using FluentValidation;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed class UpdateMobileModelCommandValidator : AbstractValidator<UpdateMobileModelCommand>
{
    public UpdateMobileModelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
