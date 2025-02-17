namespace WhoCame.Accounts.Contracts.Requests;

public record RegisterUserRequest(
    string Email,
    string Password);
