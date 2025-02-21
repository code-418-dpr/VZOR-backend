using Microsoft.Extensions.Logging;
using OpenCvSharp;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Errors;
using WhoCame.Vision.Application.Services;

namespace WhoCame.Vision.Infrastructure.Services;

/// <summary>
/// Сервис для работы с потоком видео с камер
/// </summary>
public class RemoteVideoCameraService: IRemoteVideoCameraService
{
    private readonly ILogger<RemoteVideoCameraService> _logger;
    private CancellationTokenSource _cancellationTokenSource;

    public RemoteVideoCameraService(ILogger<RemoteVideoCameraService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Начало стриминга с видео-камеры и отправка потока данных по gRPC
    /// </summary>
    /// <param name="videoUrl">Ссылка на видео</param>
    /// <returns></returns>
    public async Task<Result> StartVideoStreamingAsync(string videoUrl)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        
        using var videoCapture = new VideoCapture(videoUrl);

        if (!videoCapture.IsOpened())
        {
            _logger.LogError("Video capture is not opened");
            return Error.Failure("video.is.not.opened","Video Capture is not opened.");
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var frame = new Mat();
            videoCapture.Read(frame);
            
            if (!frame.Empty())
            {
                //TODO: Отправка по gRPC
                //await _videoStreamingClient.SendVideoFrameAsync(frame.ToBytes());
            }

            await Task.Delay(TimeSpan.FromMilliseconds(33), cancellationToken);
        }
        
        return Result.Success();
    }

    /// <summary>
    /// Остановка стриминга
    /// </summary>
    /// <returns></returns>
    public void StopVideoStreaming()
    {
        _cancellationTokenSource?.Cancel();
    }
}