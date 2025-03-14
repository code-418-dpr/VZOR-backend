using FluentValidation;

namespace VZOR.Images.Application.Features.Queries.GetImageById;

public class GetImageByIdValidator: AbstractValidator<GetImageByIdQuery>
{
    public GetImageByIdValidator()
    {
        RuleFor(c => c.ImageId)
            .NotEmpty()
            .WithMessage("Please specify an image ID.");
    }
}