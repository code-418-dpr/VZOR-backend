using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Application.Features.Commands.ChangeUserData;

public class ChangeUserDataValidator: AbstractValidator<ChangeUserDataCommand>
{
    public ChangeUserDataValidator()
    {
        RuleFor(c => c.Name)
            .MaximumLength(Constraints.MAX_VALUE_LENGTH)
            .WithError(Errors.General.ValueIsInvalid("name"));
        
        RuleFor(c => c.CurrentPassword)
            .Matches(Constraints.ValidationPassword)
            .WithError(Errors.General.ValueIsInvalid("current_password"));
        
        RuleFor(c => c.NewPassword)
            .Matches(Constraints.ValidationPassword)
            .WithError(Errors.General.ValueIsInvalid("new_password"));
    }
}