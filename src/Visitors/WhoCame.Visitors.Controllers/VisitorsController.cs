using Microsoft.AspNetCore.Mvc;
using WhoCame.Framework;
using WhoCame.Visitors.Application.Features.Commands.AddVisitor;
using WhoCame.Visitors.Application.Features.Commands.DeleteVisitor;
using WhoCame.Visitors.Contracts.Requests;

namespace WhoCame.Visitors.Controllers;

public class VisitorsController: ApplicationController
{
    [HttpPost("CreationVisitor")]
    public async Task<IActionResult> CreateVisitor(
        [FromBody] AddVisitorRequest request,
        [FromServices] AddVisitorHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddVisitorCommand(request.FirstName, request.LastName, request.MiddleName);
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();

        return Ok(result);
    }
    
    [HttpDelete("DeletionVisitor/{id:guid}")]
    public async Task<IActionResult> DeleteVisitor(
        [FromRoute] Guid id,
        [FromServices] DeleteVisitorHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteVisitorCommand(id);
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();

        return Ok(result);
    }
}