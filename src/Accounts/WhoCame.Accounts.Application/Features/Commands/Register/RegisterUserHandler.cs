using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WhoCame.Accounts.Application.Database;
using WhoCame.Accounts.Application.Managers;
using WhoCame.Accounts.Domain;
using WhoCame.Core.Abstractions;
using WhoCame.Core.Extension;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Constraints;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Accounts.Application.Features.Commands.Register;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountManager _accountManager;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterUserCommand> _validator;
    
    public RegisterUserHandler(
        UserManager<User> userManager,
        ILogger<RegisterUserHandler> logger,
        IValidator<RegisterUserCommand> validator,
        RoleManager<Role> roleManager, 
        IAccountManager accountManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
        _roleManager = roleManager;
        _accountManager = accountManager;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == ParticipantAccount.Participant, cancellationToken);
            
            if (role is null)
                return Errors.General.NotFound();

            var user = User.CreateParticipant( command.Email, role);

            var result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
                Error.Failure("cannot.create.user","Can not create user");

            var participantAccount = new ParticipantAccount(user);
            
            await _accountManager.CreateParticipantAccount(participantAccount, cancellationToken);

            user.ParticipantAccount = participantAccount;
            user.ParticipantAccountId = participantAccount.Id;

            await _unitOfWork.SaveChanges(cancellationToken);
            
            transaction.Commit();
                
            _logger.LogInformation("User created: a new account with password");

            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError("Registration of user fall with error");
            
            transaction.Rollback();
            
            return Error.Failure("cannot.create.user","Can not create user");
        }
    }
}