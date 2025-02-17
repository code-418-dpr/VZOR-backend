using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Constraints;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Accounts.Application.Features.Commands.Login;

public class LoginUserCommandValidator: AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .Must(r => Constraints.ValidationRegex.IsMatch(r))
            .WithError(Errors.General.ValueIsInvalid("email"));

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(Constraints.MIN_LENGTH_PASSWORD)
            .WithError(Errors.General.ValueIsInvalid("password"));
    }
}