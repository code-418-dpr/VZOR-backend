namespace VZOR.Accounts.Contracts.Requests;

public record RegisterUserRequest(
    string Name,
    string Email,
    string Password);
