using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Application.Features.Commands.Register;

public class RegisterUserCommandValidator: AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
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