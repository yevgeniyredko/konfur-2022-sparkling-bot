using System.Collections.Concurrent;

namespace Konfur2022SparklingBot.Common.EventLoop;

public class EventLoop
{
    private readonly ConcurrentQueue<Func<Task>> _callbacks = new();
    private readonly ILogger<EventLoop> _logger;

    public EventLoop(ILogger<EventLoop> logger) => _logger = logger;
    
    public void Push(Func<Task> callback) => _callbacks.Enqueue(callback);

    public async Task ExecuteCurrentEventsAsync()
    {
        while (_callbacks.TryDequeue(out var callback))
        {
            try
            {
                await callback.Invoke();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }
        }
    }
}