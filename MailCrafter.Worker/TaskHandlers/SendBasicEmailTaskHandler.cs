using MailCrafter.Domain;
using MailCrafter.Services;
using System.Text.Json;

namespace MailCrafter.Worker;
public class SendBasicEmailTaskHandler : ITaskHandler
{
    public string TaskName => WorkerTaskNames.Send_Basic_Email;

    private readonly ILogger<SendBasicEmailTaskHandler> _logger;
    private readonly IEmailSendingService _emailSendingService;
    public SendBasicEmailTaskHandler(
        ILogger<SendBasicEmailTaskHandler> logger,
        IEmailSendingService emailSendingService)
    {
        _logger = logger;
        _emailSendingService = emailSendingService;
    }
    public async Task HandleAsync(JsonElement payload)
    {
        var details = JsonSerializer.Deserialize<BasicEmailDetailsModel>(payload);
        if (details != null)
        {
            await _emailSendingService.SendBasicEmailsAsync(details);
        }
        else
        {
            _logger.LogError($"Cannot deserialize task payload. Task name:{WorkerTaskNames.Send_Basic_Email}");
        }
    }
}
