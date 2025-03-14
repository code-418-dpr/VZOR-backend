using System.Net.Http.Headers;
using AspNet.Security.OAuth.Yandex;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    #region auth
    
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
    
    #endregion
    
    #region oauth
    
    [HttpGet("yandex-login")]
    public IActionResult YandexLogin(CancellationToken cancellationToken = default)
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/api/Account/yandex-callback" }, "Yandex");
    }
    

    [HttpGet("yandex-callback")]
    public async Task<IActionResult> YandexCallback(string code, string state)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Yandex");

        if (!authenticateResult.Succeeded)
            return BadRequest(); // Handle error

        var accessToken = authenticateResult.Properties.GetTokenValue("access_token");

        // Здесь вы можете использовать accessToken для получения данных пользователя
        // или сохранить его в базе данных.

        return Redirect("/");
    }
    
    private async Task<string?> ExchangeCodeForToken(string code)
    {
        var redirectUri = Url.Action("YandexCallback", "Account", null, Request.Scheme);

        using var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code }, 
            { "client_id", "7b6960e71e8a4a78acff0880aaf0d373" },
            { "client_secret", "82ea36e876104a7f9f2199c04933d248" },
            { "redirect_uri", redirectUri }
        });

        
        var response = await httpClient.PostAsync("https://oauth.yandex.ru/token", content);

        if (!response.IsSuccessStatusCode)
        {
            return null; 
        }
        
        var responseData = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<YandexTokenResponse>(responseData);

        if (tokenResponse?.AccessToken != null) 
            return tokenResponse.AccessToken;
        else
            return null;
    }
    
    
    private async Task<YandexUserInfo> GetUserInfo(string accessToken)
    {
        using var httpClient = new HttpClient();
        
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", accessToken);
        
        var response = await httpClient.GetAsync("https://login.yandex.ru/info");

        if (!response.IsSuccessStatusCode)
        {
            return null; 
        }
        
        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<YandexUserInfo>(responseData);
    }
    
    
    #endregion
    
}

