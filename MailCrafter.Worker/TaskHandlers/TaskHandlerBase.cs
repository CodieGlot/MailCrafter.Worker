using System.Text.Json;

namespace MailCrafter.Worker;
public abstract class TaskHandlerBase<TModel> : ITaskHandler
{
    public abstract string TaskName { get; }

    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly ILogger _logger;
    protected TaskHandlerBase(IServiceScopeFactory serviceScopeFactory, ILogger logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task HandleAsync(JsonElement payload)
    {
        var details = JsonSerializer.Deserialize<TModel>(payload);
        if (details != null)
        {
            await HandleTaskAsync(details);
        }
        else
        {
            _logger.LogError($"Cannot deserialize task payload. Task name: {this.TaskName}");
        }
    }
    protected abstract Task HandleTaskAsync(TModel model);
}
