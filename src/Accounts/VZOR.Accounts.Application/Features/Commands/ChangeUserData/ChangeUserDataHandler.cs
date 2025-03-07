using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Accounts.Domain;
using VZOR.Core.Abstractions;
using VZOR.Core.Database;
using VZOR.Core.Extension;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Application.Features.Commands.ChangeUserData;

public class ChangeUserDataHandler: ICommandHandler<ChangeUserDataCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeUserDataHandler> _logger;
    private readonly IValidator<ChangeUserDataCommand> _validator;

    public ChangeUserDataHandler(
        UserManager<User> userManager,
        [FromKeyedServices(Constraints.Contexts.AuthContext)]IUnitOfWork unitOfWork,
        ILogger<ChangeUserDataHandler> logger,
        IValidator<ChangeUserDataCommand> validator)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(ChangeUserDataCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command,cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user is null)
            return Errors.General.NotFound(command.UserId);

        if (user.ParticipantAccount is not null && command.Name is not null)
            user.ParticipantAccount.Name = command.Name;

        if (command.CurrentPassword is not null && command.NewPassword is not null)
        {
            await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
        }

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Data of user with id {userId} has been updated", command.UserId);
        
        return Result.Success();
    }
}