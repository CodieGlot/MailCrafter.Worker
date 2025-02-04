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

    public Worker()
    {
        _taskHandlers = new Dictionary<string, ITaskHandler>
        {
            { WorkerTaskNames.Send_Email, new SendEmailTaskHandler() },
        };
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
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _channel.BasicGetAsync("task_queue", autoAck: false);
                    if (result != null)
                    {
                        var message = Encoding.UTF8.GetString(result.Body.ToArray());
                        var task = JsonSerializer.Deserialize<WorkerTaskMessage>(message);

                        await this.ProcessTaskAsync(task);
                        await _channel.BasicAckAsync(result.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        await Task.Delay(500, stoppingToken); // Wait before checking again
                    }
                }
                catch (OperationInterruptedException ex)
                {
                    Console.WriteLine($"RabbitMQ error: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing RabbitMQ: {ex.Message}");
        }
    }

    private async Task ProcessTaskAsync(WorkerTaskMessage task)
    {
        if (_taskHandlers.TryGetValue(task.TaskName, out var handler))
        {
            await handler.HandleAsync(task.Payload);
        }
        else
        {
            Console.WriteLine($"Unknown task sent to the queue: {task.TaskName}");
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
