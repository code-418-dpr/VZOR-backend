using FluentValidation;
using WhoCame.Core.Validators;
using WhoCame.SharedKernel.Constraints;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Visitors.Application.Features.Commands.AddPhotosToVisitor;

public class AddPhotosToVisitorValidator: AbstractValidator<AddPhotosToVisitorCommand>
{
    public AddPhotosToVisitorValidator()
    {
        RuleFor(c => c.VisitorId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("VisitorId"));

        RuleForEach(c => c.FileDtos)
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