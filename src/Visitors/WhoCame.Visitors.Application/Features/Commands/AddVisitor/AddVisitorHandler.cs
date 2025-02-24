using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WhoCame.Core.Abstractions;
using WhoCame.Core.Database;
using WhoCame.Core.Extension;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Constraints;
using WhoCame.Visitors.Application.Repositories;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Application.Features.Commands.AddVisitor;

public class AddVisitorHandler: ICommandHandler<AddVisitorCommand>
{
    private readonly ILogger<AddVisitorHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVisitorRepository _visitorRepository;
    private readonly IValidator<AddVisitorCommand> _validator;

    public AddVisitorHandler(
        ILogger<AddVisitorHandler> logger,
        [FromKeyedServices(Constraints.Contexts.VisitorsContext)]IUnitOfWork unitOfWork,
        IVisitorRepository visitorRepository,
        IValidator<AddVisitorCommand> validator)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _visitorRepository = visitorRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(AddVisitorCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var id = Guid.NewGuid();
        
        var visitor = new Visitor
        {
            Id = id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            MiddleName = command.MiddleName,
        };
        
        await _visitorRepository.AddAsync(visitor, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);
        
        _logger.LogInformation("Visitor with id {id} has been added in database", id);

        return Result.Success();
    }
}