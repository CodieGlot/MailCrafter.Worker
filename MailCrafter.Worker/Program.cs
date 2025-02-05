using MailCrafter.Services;
using MailCrafter.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddHostedService<Worker>()
    .AddCoreDependencies()
    .AddTaskHandlers();

var host = builder.Build();
host.Run();
