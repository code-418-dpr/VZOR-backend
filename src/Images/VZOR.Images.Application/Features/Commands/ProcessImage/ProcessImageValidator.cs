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

        RuleForEach(c => c.Images)
            .ChildRules(f =>
            {
                f.RuleFor(c => c.FileName)
                    .Must(ext => Constraints.Extensions.Contains(Path.GetExtension(ext)))
                    .NotEmpty()
                    .WithError(Error.Null("filename.is.null", "filename cannot be null or empty"));

                f.RuleFor(c => c.Content)
                    .Must(s => s.Length is > 0 and <= 15 * 1024 * 1024)
                    .WithError(Error.Null("stream.empty", "stream cannot be empty"));
            });
    }
}