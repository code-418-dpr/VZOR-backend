using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WhoCame.Core.Abstractions;
using WhoCame.Core.Database;
using WhoCame.Core.Extension;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Constraints;
using WhoCame.SharedKernel.Errors;
using WhoCame.Visitors.Application.FileProvider;
using WhoCame.Visitors.Application.FileProviders;
using WhoCame.Visitors.Application.Repositories;
using FileInfo = WhoCame.Visitors.Application.FileProvider.FileInfo;

namespace WhoCame.Visitors.Application.Features.Commands.AddPhotosToVisitor;

public class AddPhotosToVisitorHandler: ICommandHandler<AddPhotosToVisitorCommand>
{
    public const string BUCKET_NAME = "whocame";
    
    private readonly IVisitorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddPhotosToVisitorHandler> _logger;
    private readonly IValidator<AddPhotosToVisitorCommand> _validator;
    private readonly IFileProvider _fileProvider;

    public AddPhotosToVisitorHandler(
        IVisitorRepository repository,
        [FromKeyedServices(Constraints.Contexts.VisitorsContext)]IUnitOfWork unitOfWork,
        ILogger<AddPhotosToVisitorHandler> logger,
        IValidator<AddPhotosToVisitorCommand> validator,
        IFileProvider fileProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
        _fileProvider = fileProvider;
    }

    //TODO: Ссылки на фотки почему-то не сохраняются в бд(возможно проблема в конфигурации)
    public async Task<Result> Handle(AddPhotosToVisitorCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();
        
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var visitor = await _repository.GetByIdAsync(command.VisitorId, cancellationToken);
            if(visitor is null)
                return Errors.General.NotFound(command.VisitorId);

            var filesData = command.FileDtos.Select(f => 
                new FileData(f.Content, new FileInfo(f.FileName, BUCKET_NAME)));

            var paths = await _fileProvider.UploadFiles(filesData, cancellationToken);
            if(paths.IsFailure)
                return paths.Errors;
            
            var result = visitor.AddPhotos(paths.Value);
            if(result.IsFailure)
                return result.Errors;
            
            await _unitOfWork.SaveChanges(cancellationToken);
            
            transaction.Commit();
            
            _logger.LogInformation("Added photos to visitor with id {id}", command.VisitorId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            
            _logger.LogError(ex, ex.Message);

            return Error.Failure("add.photos.error", "Cannot add photos to visitors");
        }
    }
}