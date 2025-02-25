using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Visitors.Application.Features.Queries.GetVisitorById;

public class GetVisitorByIdValidator: AbstractValidator<GetVisitorByIdQuery>
{
    public GetVisitorByIdValidator()
    {
        RuleFor(q => q.VisitorId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("visitorId"));
    }
}