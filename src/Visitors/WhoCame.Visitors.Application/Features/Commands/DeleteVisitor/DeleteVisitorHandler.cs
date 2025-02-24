using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WhoCame.Core.Abstractions;
using WhoCame.Core.Database;
using WhoCame.Core.Extension;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Constraints;
using WhoCame.Visitors.Application.Repositories;

namespace WhoCame.Visitors.Application.Features.Commands.DeleteVisitor;

public class DeleteVisitorHandler: ICommandHandler<DeleteVisitorCommand>
{
    private readonly IVisitorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteVisitorHandler> _logger;
    private readonly IValidator<DeleteVisitorCommand> _validator;

    public DeleteVisitorHandler(
        IVisitorRepository repository,
        [FromKeyedServices(Constraints.Contexts.VisitorsContext)]IUnitOfWork unitOfWork,
        ILogger<DeleteVisitorHandler> logger,
        IValidator<DeleteVisitorCommand> validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(DeleteVisitorCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();

        var result = await _repository.DeleteAsync(command.Id, cancellationToken);
        if (result.IsFailure)
            return result.Errors;

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Visitor deleted with id {id}", command.Id);

        return Result.Success();
    }
}