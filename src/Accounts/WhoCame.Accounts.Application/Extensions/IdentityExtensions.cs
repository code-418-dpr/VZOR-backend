using Microsoft.AspNetCore.Identity;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Accounts.Application.Extensions;

public static class IdentityExtensions
{
    public static ErrorList ToErrorList(this IEnumerable<IdentityError> identityErrors)
    {
        var errors = identityErrors.Select(ie => Error.Failure(ie.Code, ie.Description));
        
        return new ErrorList(errors);
    }
}