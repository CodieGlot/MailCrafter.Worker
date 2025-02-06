using MailCrafter.Domain;
using MailCrafter.Services;

namespace MailCrafter.Worker;
public class SendPersonalizedEmailTaskHandler : TaskHandlerBase<PersonalizedEmailDetailsModel>
{
    public override string TaskName => WorkerTaskNames.Send_Personailized_Email;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SendPersonalizedEmailTaskHandler(
        ILogger<SendPersonalizedEmailTaskHandler> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task HandleTaskAsync(PersonalizedEmailDetailsModel model)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var emailSendingService = scope.ServiceProvider.GetRequiredService<IEmailSendingService>();
        return emailSendingService.SendPersonalizedEmailsAsync(model);
    }
}
