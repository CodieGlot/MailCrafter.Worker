using System.Text.Json;

namespace MailCrafter.Worker;
public interface ITaskHandler
{
    Task HandleAsync(JsonElement payload);
}
