using System.Security.Claims;
using WhoCame.Accounts.Application.Models;
using WhoCame.Accounts.Domain;
using WhoCame.SharedKernel;

namespace WhoCame.Accounts.Application;

public interface ITokenProvider
{
    Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken = default);
    Task<Guid> GenerateRefreshToken(User user, Guid accessTokenJti, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Claim>>> GetUserClaimsFromJwtToken(string jwtToken, CancellationToken cancellationToken = default);
}