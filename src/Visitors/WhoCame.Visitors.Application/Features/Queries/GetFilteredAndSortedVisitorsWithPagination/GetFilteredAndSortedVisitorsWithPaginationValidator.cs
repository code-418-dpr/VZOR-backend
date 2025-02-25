using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Visitors.Application.Features.Queries.GetFilteredAndSortedVisitorsWithPagination;

public class GetFilteredAndSortedVisitorsWithPaginationValidator:
    AbstractValidator<GetFilteredAndSortedVisitorsWithPaginationQuery>
{
    public GetFilteredAndSortedVisitorsWithPaginationValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThanOrEqualTo(1)
            .WithError(Errors.General.ValueIsInvalid("page"));
        
        RuleFor(q => q.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithError(Errors.General.ValueIsInvalid("page size"));
    }
}