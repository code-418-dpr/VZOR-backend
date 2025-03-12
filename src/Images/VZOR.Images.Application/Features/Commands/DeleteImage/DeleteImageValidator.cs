using FluentValidation;
using VZOR.Core.Abstractions;

namespace VZOR.Images.Application.Features.Commands.DeleteImage;

public class DeleteImageValidator: AbstractValidator<DeleteImageCommand>
{
    public DeleteImageValidator()
    {
        RuleFor(c => c.ImageId)
            .NotEmpty()
            .WithMessage("Please specify a valid image Id");
        
        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("Please specify a valid user Id");
    }
}
