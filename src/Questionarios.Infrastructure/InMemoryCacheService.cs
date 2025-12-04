using System.Collections.Concurrent;
using Questionarios.Application.Abstractions;

namespace Questionarios.Infrastructure;

public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, (object value, DateTime expiresAt)> _store = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_store.TryGetValue(key, out var entry))
        {
            if (entry.expiresAt > DateTime.UtcNow && entry.value is T typed)
                return Task.FromResult<T?>(typed);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        _store[key] = (value!, DateTime.UtcNow.Add(ttl));
        return Task.CompletedTask;
    }
}
