using FluentValidation;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed class CreateMobileBrandCommandValidator : AbstractValidator<CreateMobileBrandCommand>
{
    public CreateMobileBrandCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
