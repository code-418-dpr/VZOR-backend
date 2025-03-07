using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Application.Features.Commands.Refresh;

public class RefreshTokensCommandValidator: AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("refresh token"));
    }
}