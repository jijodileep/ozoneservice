using FluentValidation;
using OzoneMobileService.Application.Features.Users.Commands;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Application.Features.Users.Commands;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(TenantAssignableRoles.IsAssignable)
            .WithMessage("Role is not assignable.");

        RuleFor(x => x.BranchIds)
            .NotEmpty()
            .WithMessage("At least one branch must be assigned.");
    }
}
