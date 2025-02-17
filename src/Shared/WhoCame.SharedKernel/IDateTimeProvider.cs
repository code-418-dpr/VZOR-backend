namespace WhoCame.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}