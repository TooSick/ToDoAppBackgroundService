using Quartz;
using ToDoBackgoundService;
using ToDoBackgoundService.Abstractions;
using ToDoBackgoundService.AsincDataListeners;
using ToDoBackgoundService.Models;
using ToDoBackgoundService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

builder.Services.AddSingleton<IMessageBusListener, RabbitMqListener>();
builder.Services.AddSingleton<ISchedulerService<ToDoItem>, QuartzSchedulerService>();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<ToDoCleanerService>();

var host = builder.Build();
host.Run();
