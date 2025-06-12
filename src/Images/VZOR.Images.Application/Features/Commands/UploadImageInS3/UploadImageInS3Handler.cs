using FluentValidation;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Extension;
using VZOR.Images.Application.FileModels;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Jobs;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Contracts.Responses;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Commands.UploadImageInS3;

public class UploadImageInS3Handler : ICommandHandler<UploadImageInS3Command, UploadImageInS3Response>
{
    public const string BUCKET_NAME = "vzor";
    private readonly ILogger<UploadImageInS3Handler> _logger;
    private readonly IImageRepository _imageRepository;
    private readonly IS3FileProvider _s3FileProvider;
    private readonly IValidator<UploadImageInS3Command> _validator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadImageInS3Handler(
        ILogger<UploadImageInS3Handler> logger,
        [FromKeyedServices(Constraints.Database.ElasticSearch)]
        IImageRepository imageRepository,
        IS3FileProvider s3FileProvider,
        IValidator<UploadImageInS3Command> validator,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _imageRepository = imageRepository;
        _s3FileProvider = s3FileProvider;
        _validator = validator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<UploadImageInS3Response>> Handle(
        UploadImageInS3Command command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var s3Files = new List<FileMetadataS3>();
        var images = new List<Image>();

        foreach (var file in command.Files)
        {
            var id = Guid.NewGuid();

            var uploadLink = id + Path.GetExtension(file.FileName);

            s3Files.Add(new FileMetadataS3
            {
                BucketName = BUCKET_NAME,
                Key = uploadLink,
                FileName = file.FileName,
                ContentType = file.ContentType,
            });

            images.Add(new Image
            {
                Id = id.ToString(),
                UserId = command.UserId.ToString(),
                UploadLink = uploadLink,
                UploadDate = _dateTimeProvider.UtcNow
            });
        }

        await _imageRepository.AddRangeAsync(images, cancellationToken);

        var uploadUrls = await _s3FileProvider.GetPresignedUrlsForUploadParallel(s3Files, cancellationToken);
        if (uploadUrls.IsFailure)
            return uploadUrls.Errors;

        _logger.LogInformation("Uploaded {Count} files.", s3Files.Count);

        var ids = images.Select(x => x.Id).ToList();
        var uploadLinks = images.Select(x => x.UploadLink).ToList();

        var jobId = BackgroundJob.Schedule<ConfirmConsistencyJob>(
            j => j.Execute(
                ids, BUCKET_NAME, uploadLinks),
            TimeSpan.FromMinutes(3));


        var response = new UploadImageInS3Response(uploadUrls.Value.Zip(images,
            (url, image) => new UploadImageUrl(Guid.Parse(image.Id), url)));

        return response;
    }
}