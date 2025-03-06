namespace VZOR.Accounts.Contracts.Requests;

public record RegisterUserRequest(
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string Password);
