using Microsoft.AspNetCore.Mvc;
using VZOR.Framework;
using VZOR.Framework.Authorization;
using VZOR.Framework.Models;
using VZOR.Framework.Processors;
using VZOR.Images.Application.Features.UploadImage;
using VZOR.Images.Controllers.Requests;

namespace VZOR.Images.Controllers;

public class ImagesController: ApplicationController
{
    [Permission("update")]
    [HttpPost]
    public async Task<ActionResult> UploadPhotos(
        [FromForm]UploadImagesRequest request,
        [FromServices] UserScopedData userScopedData,
        [FromServices] UploadImageHandler handler,
        CancellationToken cancellationToken = default)
    {
        await using var fileProcessor = new FormFileProcessor();

        var fileDtos = fileProcessor.Process(request.Files);

        var command = new UploadImageCommand(userScopedData.UserId, fileDtos);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Errors.ToResponse();
        
        return Ok();
    }
}