using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoCame.Core.Abstractions;
using WhoCame.Core.Extension;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Errors;
using WhoCame.Visitors.Application.Database;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Application.Features.Queries.GetVisitorById;

public class GetVisitorByIdHandler: IQueryHandler<Visitor,GetVisitorByIdQuery>
{
    private readonly ILogger<GetVisitorByIdHandler> _logger;
    private readonly IValidator<GetVisitorByIdQuery> _validator;
    private readonly IReadDbContext _readDbContext;

    public GetVisitorByIdHandler(
        ILogger<GetVisitorByIdHandler> logger,
        IValidator<GetVisitorByIdQuery> validator,
        IReadDbContext readDbContext)
    {
        _logger = logger;
        _validator = validator;
        _readDbContext = readDbContext;
    }

    public async Task<Result<Visitor>> Handle(
        GetVisitorByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var visitor = await _readDbContext.Visitors
            .FirstOrDefaultAsync(v => v.Id == query.VisitorId, cancellationToken);

        if (visitor is null)
            return Errors.General.NotFound();
        
        _logger.LogInformation("Get visitor by id: {Id}", visitor.Id);
        
        return visitor;
    }
}