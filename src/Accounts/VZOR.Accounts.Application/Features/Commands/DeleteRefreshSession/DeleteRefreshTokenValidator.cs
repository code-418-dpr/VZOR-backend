using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Application.Features.Commands.DeleteRefreshSession;

public class DeleteRefreshTokenValidator: AbstractValidator<DeleteRefreshTokenCommand>
{
    public DeleteRefreshTokenValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("refresh token"));
    }
}