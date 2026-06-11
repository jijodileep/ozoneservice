using FluentValidation;
using OzoneMobileService.Application.Features.Users.Commands;

namespace OzoneMobileService.Application.Features.Users.Commands;

public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}
