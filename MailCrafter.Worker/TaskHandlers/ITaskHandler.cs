using System.Text.Json;

namespace MailCrafter.Worker;
public interface ITaskHandler
{
    string TaskName { get; }
    Task HandleAsync(JsonElement payload);
}
