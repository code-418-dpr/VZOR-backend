using WhoCame.SharedKernel;

namespace WhoCame.Vision.Application.Services;

public interface IRemoteVideoCameraService
{
    Task<Result> StartVideoStreamingAsync(string videoUrl);
    void StopVideoStreaming();
}