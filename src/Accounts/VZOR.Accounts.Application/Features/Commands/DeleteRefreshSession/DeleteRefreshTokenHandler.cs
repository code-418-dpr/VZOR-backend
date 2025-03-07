using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Accounts.Application.Managers;
using VZOR.Core.Abstractions;
using VZOR.Core.Database;
using VZOR.Core.Extension;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;

namespace VZOR.Accounts.Application.Features.Commands.DeleteRefreshSession;

public class DeleteRefreshTokenHandler: ICommandHandler<DeleteRefreshTokenCommand>
{
    private readonly ILogger<DeleteRefreshTokenHandler> _logger;
    private readonly IValidator<DeleteRefreshTokenCommand> _validator;
    private readonly IRefreshSessionManager _refreshSessionManager;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRefreshTokenHandler(
        ILogger<DeleteRefreshTokenHandler> logger,
        IValidator<DeleteRefreshTokenCommand> validator,
        IRefreshSessionManager refreshSessionManager,
        [FromKeyedServices(Constraints.Contexts.AuthContext)]IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _refreshSessionManager = refreshSessionManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteRefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();

        var refreshSession = await _refreshSessionManager
            .GetByRefreshToken(command.RefreshToken, cancellationToken);
        
        if (refreshSession.IsFailure)
            return refreshSession.Errors;
        
        _refreshSessionManager.Delete(refreshSession.Value);

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("RefreshSession has been deleted");
        
        return Result.Success();
    }
}