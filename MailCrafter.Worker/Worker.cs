using MailCrafter.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace MailCrafter.Worker;

public class Worker : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly Dictionary<string, ITaskHandler> _taskHandlers;
    private readonly ILogger<Worker> _logger;

    // Inject Task Handlers and Logger through DI
    public Worker(IEnumerable<ITaskHandler> taskHandlers, ILogger<Worker> logger)
    {
        _logger = logger;
        _taskHandlers = taskHandlers.ToDictionary(handler => handler.TaskName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        try
        {
            // Retry logic for connecting to RabbitMQ
            await ConnectToRabbitMqAsync(factory, stoppingToken);

            // Start consuming messages from the queue
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_channel == null)
                    {
                        _logger.LogError("Channel is null. Cannot consume messages.");
                        break;
                    }

                    var result = await _channel.BasicGetAsync("worker_task_queue", autoAck: false);
                    if (result != null)
                    {
                        var message = Encoding.UTF8.GetString(result.Body.ToArray());
                        var task = JsonSerializer.Deserialize<WorkerTaskMessage>(message);

                        await ProcessTaskAsync(task);
                        await _channel.BasicAckAsync(result.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        await Task.Delay(500, stoppingToken);
                    }
                }
                catch (OperationInterruptedException ex)
                {
                    _logger.LogError($"RabbitMQ error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error happens when executing task: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error connecting to RabbitMQ: {ex.Message}");
        }
    }

    // Retry logic for connecting to RabbitMQ
    private async Task ConnectToRabbitMqAsync(ConnectionFactory factory, CancellationToken stoppingToken)
    {
        int retryCount = 5;
        while (retryCount > 0)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    "worker_task_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                break;
            }
            catch (Exception ex)
            {
                retryCount--;
                _logger.LogError($"Error connecting to RabbitMQ: {ex.Message}. Retries left: {retryCount}");
                if (retryCount == 0)
                    throw;
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private async Task ProcessTaskAsync(WorkerTaskMessage? task)
    {
        if (task != null && _taskHandlers.TryGetValue(task.TaskName, out var handler))
        {
            await handler.HandleAsync(task.Payload);
        }
        else
        {
            _logger.LogWarning($"Unknown task sent to the queue: {task?.TaskName}");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
