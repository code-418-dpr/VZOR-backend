using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Commands.UploadImage;

public class ProcessImageValidator : AbstractValidator<ProcessImageCommand>
{
    public ProcessImageValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("user_id"));

        RuleForEach(c => c.FileIds)
            .ChildRules(f =>
            {
                f.RuleFor(c => c)
                    .NotEmpty()
                    .WithError(Error.Null("file.id.empty", "file id cannot be empty"));
            });
    }
}