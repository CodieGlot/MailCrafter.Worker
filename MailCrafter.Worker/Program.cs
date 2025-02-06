using MailCrafter.Services;
using MailCrafter.Worker;

var configuration = new ConfigurationBuilder()
    .AddJsonFile(@"C:\MailCrafter\Development\Core\appsettings.Development.json", optional: false, reloadOnChange: true)
    .Build();

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddHostedService<Worker>()
    .AddSingleton<IConfiguration>(configuration)
    .AddCoreServices()
    .AddTaskHandlers();

var host = builder.Build();
host.Run();
