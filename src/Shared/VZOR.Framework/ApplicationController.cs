using Microsoft.AspNetCore.Mvc;
using VZOR.Core.Models;

namespace VZOR.Framework;

[ApiController]
[Route("api/[controller]")]
public abstract class ApplicationController: ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);

        return base.Ok(envelope);
    }
    
    
}