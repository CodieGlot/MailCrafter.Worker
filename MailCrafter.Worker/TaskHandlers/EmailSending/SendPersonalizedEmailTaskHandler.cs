using MailCrafter.Domain;
using MailCrafter.Services;

namespace MailCrafter.Worker;
public class SendPersonalizedEmailTaskHandler : TaskHandlerBase<PersonalizedEmailDetailsModel>
{
    public override string TaskName => WorkerTaskNames.Send_Personailized_Email;

    private readonly IEmailSendingService _emailSendingService;

    public SendPersonalizedEmailTaskHandler(
        ILogger<SendPersonalizedEmailTaskHandler> logger,
        IEmailSendingService emailSendingService)
        : base(logger)
    {
        _emailSendingService = emailSendingService;
    }

    protected override Task HandleTaskAsync(PersonalizedEmailDetailsModel model)
    {
        return _emailSendingService.SendPersonalizedEmailsAsync(model);
    }
}
