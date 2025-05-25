using MailCrafter.Domain;
using MailCrafter.Services;

namespace MailCrafter.Worker;
public class SendBasicEmailTaskHandler : TaskHandlerBase<BasicEmailDetailsModel>
{
    public override string TaskName => WorkerTaskNames.Send_Basic_Email;

    public SendBasicEmailTaskHandler(
        ILogger<SendBasicEmailTaskHandler> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory, logger)
    {

    }

    protected override Task HandleTaskAsync(BasicEmailDetailsModel model)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var emailSendingService = scope.ServiceProvider.GetRequiredService<IEmailSendingService>();
        return emailSendingService.SendBasicEmailsAsync(model);
    }
}