using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Accounts.Application.Features.Commands.Refresh;

public class RefreshTokensCommandValidator: AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(r => r.AccessToken)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("access token"));
        
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("refresh token"));
    }
}