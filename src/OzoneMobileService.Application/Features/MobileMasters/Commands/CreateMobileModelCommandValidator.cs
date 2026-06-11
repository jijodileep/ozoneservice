using FluentValidation;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed class CreateMobileModelCommandValidator : AbstractValidator<CreateMobileModelCommand>
{
    public CreateMobileModelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
