using VZOR.SharedKernel;

namespace VZOR.Core.Common;

public class DateTimeProvider: IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}