using System.Security.Claims;
using VZOR.Accounts.Application.Models;
using VZOR.Accounts.Domain;
using VZOR.SharedKernel;

namespace VZOR.Accounts.Application;

public interface ITokenProvider
{
    Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken = default);
    Task<Guid> GenerateRefreshToken(User user, Guid accessTokenJti, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Claim>>> GetUserClaimsFromJwtToken(string jwtToken, CancellationToken cancellationToken = default);
}