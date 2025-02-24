using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Visitors.Application.Features.Commands.DeleteVisitor;

public class DeleteVisitorValidator : AbstractValidator<DeleteVisitorCommand>
{
    public DeleteVisitorValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("id is empty"));
    }
}