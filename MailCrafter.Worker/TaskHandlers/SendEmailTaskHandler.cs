using System.Text.Json;

namespace MailCrafter.Worker;
public class SendEmailTaskHandler : ITaskHandler
{
    public async Task HandleAsync(JsonElement payload)
    {
        // Logic to send email
        Console.WriteLine($"Sending email with data: {payload}");
    }
}
