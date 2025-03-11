using System.Net.Http.Headers;
using AspNet.Security.OAuth.Yandex;
using Microsoft.AspNetCore.Authentication;
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
        // Указываем redirect_uri
        var redirectUri = Url.Action("YandexCallback", "Account", null, Request.Scheme);

        // Создаем свойства для Challenge
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri
        };
        
        return Challenge(YandexAuthenticationDefaults.AuthenticationScheme);
    }
    

    [HttpGet("yandex-callback")]
    public async Task<IActionResult> YandexCallback()
    {
        // Обработка callback-а от Yandex
        var result = await HttpContext.AuthenticateAsync(YandexAuthenticationDefaults.AuthenticationScheme);
        if (result.Succeeded)
        {
            // Пользователь успешно авторизован
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // Произошла ошибка при авторизации
            return RedirectToAction("Error", "Home");
        }
    }
    
    private async Task<string> ExchangeCodeForToken(string code)
    {
        var redirectUri = Url.Action("YandexCallback", "Account", null, Request.Scheme);

        using var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" }, // Тип гранта (authorization code flow)
            { "code", code }, // Код авторизации, полученный от Яндекса
            { "client_id", "7b6960e71e8a4a78acff0880aaf0d373" }, // Ваш client_id
            { "client_secret", "82ea36e876104a7f9f2199c04933d248" }, // Ваш client_secret
            { "redirect_uri", redirectUri } // URL, на который Яндекс перенаправил пользователя
        });

        // Отправляем POST-запрос к Яндекс OAuth API
        var response = await httpClient.PostAsync("https://oauth.yandex.ru/token", content);

        if (!response.IsSuccessStatusCode)
        {
            return null; // Если запрос не удался, возвращаем null
        }

        // Читаем ответ и десериализуем его
        var responseData = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<YandexTokenResponse>(responseData);

        return tokenResponse?.AccessToken; // Возвращаем access token
    }

// Модель для десериализации ответа от Яндекса
    private class YandexTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } // Access token

        [JsonProperty("token_type")]
        public string TokenType { get; set; } // Тип токена (обычно "bearer")

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } // Время жизни токена в секундах
    }
    
    private async Task<YandexUserInfo> GetUserInfo(string accessToken)
    {
        using var httpClient = new HttpClient();

        // Добавляем access token в заголовок авторизации
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", accessToken);

        // Отправляем GET-запрос к Яндекс API
        var response = await httpClient.GetAsync("https://login.yandex.ru/info");

        if (!response.IsSuccessStatusCode)
        {
            return null; // Если запрос не удался, возвращаем null
        }

        // Читаем ответ и десериализуем его
        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<YandexUserInfo>(responseData);
    }

// Модель для десериализации данных пользователя
    private class YandexUserInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; } // Уникальный ID пользователя

        [JsonProperty("login")]
        public string Login { get; set; } // Логин пользователя

        [JsonProperty("emails")]
        public List<string> Emails { get; set; } // Список email-адресов

        [JsonProperty("default_email")]
        public string DefaultEmail { get; set; } // Основной email
    }
    
    #endregion
    
}

