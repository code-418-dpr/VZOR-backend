using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Queries.GetImagesQuery;

public class GetImagesValidator : AbstractValidator<GetImagesQuery>
{
    public GetImagesValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThanOrEqualTo(1)
            .WithError(Errors.General.ValueIsInvalid("page"));
        
        RuleFor(q => q.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithError(Errors.General.ValueIsInvalid("page size"));
    }
}