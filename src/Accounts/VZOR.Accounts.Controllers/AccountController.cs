using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VZOR.Accounts.Application.Features.Commands.Login;
using VZOR.Accounts.Application.Features.Commands.Refresh;
using VZOR.Accounts.Application.Features.Commands.Register;
using VZOR.Accounts.Contracts.Requests;
using VZOR.Framework;
using VZOR.Framework.Authorization;

namespace VZOR.Accounts.Controllers;

public class AccountController: ApplicationController
{
    [Permission("create")]
    [HttpPost("registration")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request, 
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();
        
        return Ok(result.IsSuccess);
    } 
    
    [HttpPost("authentication")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request, 
        [FromServices] LoginUserHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();
        
        return Ok(result.Value);
    }

    [HttpPost("refreshing")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RefreshTokensCommand(request.AccessToken, request.RefreshToken);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();

        return Ok(result.Value);
    }
    
}