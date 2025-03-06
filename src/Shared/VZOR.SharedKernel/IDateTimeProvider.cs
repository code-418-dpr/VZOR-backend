namespace VZOR.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}