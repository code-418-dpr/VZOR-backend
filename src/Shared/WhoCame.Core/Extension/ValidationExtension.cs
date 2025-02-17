using FluentValidation.Results;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Core.Extension;

public static class ValidationExtension
{
    public static ErrorList ToErrorList(this ValidationResult validationResult)
    {
        var validationErrors = validationResult.Errors;
        
        var errors = from validationError in validationErrors
            let errorMessage = validationError.ErrorMessage
            let error = Error.Deserialize(errorMessage)
            select Error.Validation(error.ErrorCode, error.ErrorMessage, validationError.PropertyName);

        return new ErrorList(errors);
    }
}