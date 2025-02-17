namespace WhoCame.Accounts.Contracts.Responses;

public record LoginResponse(string AccessToken, Guid RefreshToken);