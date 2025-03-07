namespace VZOR.Accounts.Contracts.Requests;

public record ChangeUserDataRequest(string? Name, string? CurrentPassword, string? NewPassword);
