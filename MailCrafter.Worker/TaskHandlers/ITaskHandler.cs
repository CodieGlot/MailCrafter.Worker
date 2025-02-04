using System.Text.Json;

namespace MailCrafter.Worker;
public interface ITaskHandler
{
    void Handle(JsonElement payload);
}
