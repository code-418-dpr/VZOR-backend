using FluentValidation;
using VZOR.Core.Validators;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Commands.UploadImageInS3;

public class UploadImageInS3CommandValidator: AbstractValidator<UploadImageInS3Command>
{
    public UploadImageInS3CommandValidator()
    {
        RuleForEach(c => c.Files)
            .ChildRules(f =>
            {
                f. RuleFor(command => command)
                    .NotEmpty()
                    .WithError(Errors.General.ValueIsRequired("FileName"));

                f.RuleFor(command => command.ContentType)
                    .NotEmpty()
                    .WithError(Errors.General.ValueIsRequired("ContentType"));
            });
    }
}