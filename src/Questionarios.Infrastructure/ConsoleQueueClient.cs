using Questionarios.Application.Abstractions;

namespace Questionarios.Infrastructure;

public class ConsoleQueueClient : IQueueClient
{
    public Task PublishResponseAsync(ResponseMessage message, CancellationToken ct = default)
    {
        Console.WriteLine($"[Queue] Survey {message.SurveyId} - {message.Items.Count} items.");
        return Task.CompletedTask;
    }
}
