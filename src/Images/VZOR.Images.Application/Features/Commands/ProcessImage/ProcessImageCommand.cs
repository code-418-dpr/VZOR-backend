using VZOR.Core.Abstractions;
using VZOR.Core.Dtos;

namespace VZOR.Images.Application.Features.Commands.UploadImage;

public record ProcessImageCommand(Guid UserId, IEnumerable<Guid> FileIds): ICommand;
