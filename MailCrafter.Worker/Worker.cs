using MailCrafter.Domain;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MailCrafter.Worker;

public class Worker : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly Dictionary<string, ITaskHandler> _taskHandlers;

    public Worker()
    {
        // Initialize handlers
        _taskHandlers = new Dictionary<string, ITaskHandler>
        {
            { WorkerTaskNames.Send_Email, new SendEmailTaskHandler() },
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost:5672",
            UserName = "guest",
            Password = "guest"
        };

        // Create the connection
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare the queue
        _channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var task = JsonSerializer.Deserialize<WorkerTaskMessage>(message);

            // Process the task asynchronously
            await ProcessTaskAsync(task);

            // Acknowledge message
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: "mailcrafter-tasks-queue", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
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

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close(); // Close the connection synchronously
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
