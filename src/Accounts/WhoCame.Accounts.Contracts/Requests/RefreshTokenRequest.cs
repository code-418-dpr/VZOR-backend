namespace WhoCame.Accounts.Contracts.Requests;

public record RefreshTokenRequest(string AccessToken, Guid RefreshToken);