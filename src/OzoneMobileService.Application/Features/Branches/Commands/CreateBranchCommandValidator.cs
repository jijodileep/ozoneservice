using FluentValidation;
using OzoneMobileService.Application.Features.Branches.Commands;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Application.Features.Branches.Commands;

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Phone)
            .Must(p => PhoneNormalizer.TryNormalizeOptional(p, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Enter a valid 10-digit phone number.");

        RuleFor(x => x.GstNumber)
            .Must(g => GstinNormalizer.TryNormalizeOptional(g, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.GstNumber))
            .WithMessage("Enter a valid 15-character GSTIN.");
    }
}
