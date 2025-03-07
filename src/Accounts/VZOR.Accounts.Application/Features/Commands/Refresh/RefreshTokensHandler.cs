using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VZOR.Accounts.Application.Managers;
using VZOR.Accounts.Contracts.Responses;
using VZOR.Core.Abstractions;
using VZOR.Core.Database;
using VZOR.Core.Extension;
using VZOR.Core.Models;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;


namespace VZOR.Accounts.Application.Features.Commands.Refresh;

public class RefreshTokensHandler: ICommandHandler<RefreshTokensCommand, LoginResponse>
{
    private readonly IRefreshSessionManager _refreshSessionManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IValidator<RefreshTokensCommand> _validator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokensHandler(
        IRefreshSessionManager refreshSessionManager,
        IValidator<RefreshTokensCommand> validator,
        IDateTimeProvider dateTimeProvider,
        ITokenProvider tokenProvider,
        [FromKeyedServices(Constraints.Contexts.AuthContext)]IUnitOfWork unitOfWork)
    {
        _refreshSessionManager = refreshSessionManager;
        _validator = validator;
        _dateTimeProvider = dateTimeProvider;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<LoginResponse>> Handle(
        RefreshTokensCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();

        var refreshSession = await _refreshSessionManager
            .GetByRefreshToken(command.RefreshToken, cancellationToken);
        
        if (refreshSession.IsFailure)
            return refreshSession.Errors;

        if (refreshSession.Value.ExpiresIn < _dateTimeProvider.UtcNow)
            return Errors.Tokens.ExpiredToken();
        
        _refreshSessionManager.Delete(refreshSession.Value);
        await _unitOfWork.SaveChanges(cancellationToken);

        var accessToken = await _tokenProvider
            .GenerateAccessToken(refreshSession.Value.User,cancellationToken);
        var refreshToken = await _tokenProvider
            .GenerateRefreshToken(refreshSession.Value.User,accessToken.Jti, cancellationToken);

        return new LoginResponse(accessToken.AccessToken, refreshToken);
    }
}