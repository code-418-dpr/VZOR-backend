using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Visitors.Application.Features.Commands.AddVisitor;

public class AddVisitorValidator: AbstractValidator<AddVisitorCommand>
{
    public AddVisitorValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("firstName is empty"));
        
        RuleFor(c => c.LastName)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("lastName is empty"));
    }
}