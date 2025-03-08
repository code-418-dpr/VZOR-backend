using VZOR.Core.Abstractions;
using VZOR.Core.Dtos;

namespace VZOR.Images.Application.Features.UploadImage;

public record UploadImageCommand(Guid UserId, IEnumerable<CreateFileDto> Images): ICommand;
