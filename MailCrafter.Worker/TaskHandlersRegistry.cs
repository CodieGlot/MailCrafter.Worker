namespace MailCrafter.Worker;
public static class TaskHandlersRegistry
{
    public static IServiceCollection AddTaskHandlers(this IServiceCollection services)
    {
        services.AddTransient<ITaskHandler, SendBasicEmailTaskHandler>();
        services.AddTransient<ITaskHandler, SendPersonalizedEmailTaskHandler>();

        return services;
    }
}
