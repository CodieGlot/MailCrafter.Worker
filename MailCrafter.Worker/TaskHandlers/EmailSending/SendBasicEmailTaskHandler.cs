using MailCrafter.Domain;
using MailCrafter.Services;

namespace MailCrafter.Worker;
public class SendBasicEmailTaskHandler : TaskHandlerBase<BasicEmailDetailsModel>
{
    public override string TaskName => WorkerTaskNames.Send_Basic_Email;

    private readonly IEmailSendingService _emailSendingService;

    public SendBasicEmailTaskHandler(
        ILogger<SendBasicEmailTaskHandler> logger,
        IEmailSendingService emailSendingService)
        : base(logger)
    {
        _emailSendingService = emailSendingService;
    }

    protected override Task HandleTaskAsync(BasicEmailDetailsModel model)
    {
        return _emailSendingService.SendBasicEmailsAsync(model);
    }
}