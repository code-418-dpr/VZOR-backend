using WhoCame.SharedKernel;

namespace WhoCame.Core.Common;

public class DateTimeProvider: IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}