using VZOR.Core.Abstractions;

namespace VZOR.Images.Application.Features.Commands.DeleteImage;

public record DeleteImageCommand(Guid UserId, Guid ImageId) : ICommand;
