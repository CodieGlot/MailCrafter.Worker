namespace MailCrafter.Worker;
public static class TaskHandlerExtensions
{
    public static IServiceCollection AddTaskHandlers(this IServiceCollection services)
    {
        services.AddTransient<ITaskHandler, SendBasicEmailTaskHandler>();

        return services;
    }
}
