using Microsoft.AspNetCore.Mvc;
using VZOR.Framework;
using VZOR.Framework.Authorization;
using VZOR.Framework.Models;
using VZOR.Framework.Processors;
using VZOR.Images.Application.Features.Commands.DeleteImage;
using VZOR.Images.Application.Features.Commands.UploadImage;
using VZOR.Images.Application.Features.Queries.GetImagesQuery;
using VZOR.Images.Controllers.Requests;

namespace VZOR.Images.Controllers;

public class ImagesController: ApplicationController
{
    [Permission("update")]
    [HttpPost]
    public async Task<ActionResult> UploadImages(
        [FromForm] UploadImagesRequest request,
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
    
    [Permission("update")]
    [HttpPost("removing")]
    public async Task<ActionResult> DeleteImage(
        [FromBody] Guid imageId,
        [FromServices] UserScopedData userScopedData,
        [FromServices] DeleteImageHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteImageCommand(userScopedData.UserId, imageId);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Errors.ToResponse();
        
        return Ok();
    }
    
    [Permission("read")]
    [HttpGet]
    public async Task<ActionResult> GetImages(
        [FromQuery] GetImagesByUserIdRequest request,
        [FromServices] UserScopedData userScopedData,
        [FromServices] GetImagesHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new GetImagesQuery(
            userScopedData.UserId, 
            request.StartUploadDate,
            request.EndUploadDate,
            request.SortBy,
            request.SortDirection,
            request.Page,
            request.PageSize);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Errors.ToResponse();
        
        return Ok(result);
    }
}