using Microsoft.AspNetCore.Mvc;
using VZOR.Framework;
using VZOR.Framework.Processors;
using VZOR.Images.Controllers.Requests;

namespace VZOR.Images.Controllers;

public class VisitorsController: ApplicationController
{
    
    [HttpPost]
    public async Task<ActionResult> UploadPhotos(
        CancellationToken cancellationToken = default)
    {
        /*await using var fileProcessor = new FormFileProcessor();

        var fileDtos = fileProcessor.Process(request.Files);

        var command = new AddPhotosToVisitorCommand(visitorId, fileDtos);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Errors.ToResponse();*/
        
        return Ok();
    }
}