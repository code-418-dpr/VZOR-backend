using VZOR.Core.Abstractions;
using VZOR.Images.Contracts.Responses;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Commands.UploadImageInS3;

public class UploadImageInS3Handler: ICommandHandler<UploadImageInS3Command, UploadImageInS3Response>
{
    public async Task<Result<UploadImageInS3Response>> Handle(
        UploadImageInS3Command command, CancellationToken cancellationToken = default)
    {
        return Error.Failure("s","s");
    }
}