using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Queries.GetImagesByQuery;

public class GetImagesByQueryValidator: AbstractValidator<GetImagesByQueryQuery>
{
    public GetImagesByQueryValidator()
    {
        RuleFor(q => q.UserId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("user id"));
        
        RuleFor(q => q.Query)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("query"));
    }
}