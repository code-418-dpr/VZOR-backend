using Microsoft.AspNetCore.Identity;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Application.Extensions;

public static class IdentityExtensions
{
    public static ErrorList ToErrorList(this IEnumerable<IdentityError> identityErrors)
    {
        var errors = identityErrors.Select(ie => Error.Failure(ie.Code, ie.Description));
        
        return new ErrorList(errors);
    }
}