using FluentValidation;
using OzoneMobileService.Application.Features.Users.Commands;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Application.Features.Users.Commands;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(TenantAssignableRoles.IsAssignable)
            .WithMessage("Role is not assignable.");

        RuleFor(x => x.BranchIds)
            .NotEmpty()
            .WithMessage("At least one branch must be assigned.");
    }
}
