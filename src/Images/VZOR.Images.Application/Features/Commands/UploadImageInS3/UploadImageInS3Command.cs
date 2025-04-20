using VZOR.Core.Abstractions;
using VZOR.Images.Application.FileModels;

namespace VZOR.Images.Application.Features.Commands.UploadImageInS3;

public record UploadImageInS3Command(Guid UserId, IEnumerable<FileDataS3> Files): ICommand;
