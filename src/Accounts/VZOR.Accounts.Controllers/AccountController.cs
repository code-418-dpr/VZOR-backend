using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VZOR.Accounts.Application.Features.Commands.ChangeUserData;
using VZOR.Accounts.Application.Features.Commands.DeleteRefreshSession;
using VZOR.Accounts.Application.Features.Commands.Login;
using VZOR.Accounts.Application.Features.Commands.Refresh;
using VZOR.Accounts.Application.Features.Commands.Register;
using VZOR.Accounts.Contracts.Requests;
using VZOR.Framework;
using VZOR.Framework.Authorization;
using VZOR.Framework.Models;

namespace VZOR.Accounts.Controllers;

public class AccountController: ApplicationController
{
    [HttpPost("registration")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request, 
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand(
            request.Name,
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
        
        HttpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken.ToString());
        
        return Ok(result.Value);
    }

    [HttpPost("refreshing")]
    public async Task<IActionResult> Refresh(
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized();
        }
        
        var command = new RefreshTokensCommand(Guid.Parse(refreshToken));

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();

        return Ok(result.Value);
    }
    
    [Permission("read")]
    [HttpPost("updating")]
    public async Task<IActionResult> UpdateUserData(
        [FromForm] ChangeUserDataRequest request,
        [FromServices] UserScopedData userScopedData,
        [FromServices] ChangeUserDataHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new ChangeUserDataCommand(
            userScopedData.UserId,
            request.Name, 
            request.CurrentPassword,
            request.NewPassword);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();

        return Ok(result);
    }
    
    [HttpPost("deletion")]
    public async Task<IActionResult> Delete(
        [FromServices] DeleteRefreshTokenHandler handler,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized();
        }
        
        var command = new DeleteRefreshTokenCommand(Guid.Parse(refreshToken));

        HttpContext.Response.Cookies.Delete("refreshToken");
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Errors.ToResponse();

        return Ok(result);
    }
    
}