using MailCrafter.Domain;
using MailCrafter.Services;
using MailCrafter.Worker;

public class MVCWorker(
    MVCTaskQueueInstance taskQueue,
    IEnumerable<ITaskHandler> taskHandlers,
    ILogger<MVCWorker> logger) : BackgroundService
{
    private readonly MVCTaskQueueInstance _taskQueue = taskQueue;
    private readonly Dictionary<string, ITaskHandler> _taskHandlers = taskHandlers.ToDictionary(h => h.TaskName);
    private readonly ILogger<MVCWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MVCWorker started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var task = await _taskQueue.DequeueAsync(stoppingToken);
                await ProcessTaskAsync(task);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing task: {ex.Message}");
            }
        }
        _logger.LogInformation("MVCWorker stopped.");
    }

    private async Task ProcessTaskAsync(WorkerTaskMessage task)
    {
        if (_taskHandlers.TryGetValue(task.TaskName, out var handler))
        {
            await handler.HandleAsync(task.Payload);
        }
        else
        {
            _logger.LogWarning($"Unknown task: {task.TaskName}");
        }
    }
}
