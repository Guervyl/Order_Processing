using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tranzaksyon.Database.AppDbContext;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppSqlServerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<MonitorLoop>();
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
{
    if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
    {
        queueCapacity = 100;
    }

    return new DefaultBackgroundTaskQueue(queueCapacity);
});

//builder.Services.AddHostedService<Worker>();

var host = builder.Build();

MonitorLoop monitorLoop = host.Services.GetRequiredService<MonitorLoop>()!;
monitorLoop.StartMonitorLoop();

host.Run();
