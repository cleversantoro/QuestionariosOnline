using Questionarios.Domain.Abstractions;

namespace Questionarios.Infrastructure;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
