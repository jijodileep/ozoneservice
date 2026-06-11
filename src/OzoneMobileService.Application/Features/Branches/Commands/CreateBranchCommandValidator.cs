using FluentValidation;
using OzoneMobileService.Application.Features.Branches.Commands;

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
            .When(x => x.Address is not null);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone is not null);

        RuleFor(x => x.GstNumber)
            .MaximumLength(20)
            .When(x => x.GstNumber is not null);
    }
}
