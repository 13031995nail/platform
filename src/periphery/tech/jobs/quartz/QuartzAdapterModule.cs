namespace Tmobis.Jobs.Quartz;
using global::Quartz;
using Tmobis.Core.Modules;
using Tmobis.Jobs.Scheduled;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Mappers;

/// <summary>
/// Модуль планировщика сообщений на Quartz.
/// Сопоставляет message-типы с cron-расписаниями и создаёт Quartz jobs/triggers.
/// </summary>
[ModuleRequirement([typeof(MessageScheduleModule)])]
public sealed class QuartzAdapterModule : ModuleBase
{
    private IMessageRegister? MessageRegister { get; set; }

    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        GetMessageRegister(modules);
        RegisterQuartzJobs(services, configuration, modules);
        RegisterQuartzHost(services);
    }

    private void GetMessageRegister(IModuleRegister modules)
    {
        // TODO: DI
        MessageRegister = new MessageRegister(modules);
    }

    protected void RegisterQuartzHost(IServiceCollection services)
    {
        services.AddQuartz();
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }

    protected void RegisterQuartzJobs(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        var options = GetOptions(configuration);
        var schedules = options.Schedule;
        var scheduledMessages = GetScheduledMessages(modules, schedules);

        foreach (var message in scheduledMessages)
        {
            services.AddQuartz(q =>
            {
                var jobType = typeof(TimedMessageJob<>)
                    .MakeGenericType(message.Key);
                var jobKeyStr = message.Key.FullName;
                var jobKey = new JobKey(jobKeyStr);
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobKeyStr}-trigger")
                    .WithCronSchedule(message.Value!,
                        x => x.InTimeZone(TimeZoneInfo.Utc))
                );
                q.AddJob(jobType, jobKey, opts => opts.WithIdentity(jobKey));
            });
        }
    }

    protected ScheduledMessagesOptions GetOptions(IConfiguration configuration)
    {
        var options = new ScheduledMessagesOptions();
        configuration
            .GetSection(ScheduledMessagesOptions.ScheduledMessages)
            .Bind(options);
        return options;
    }

    protected Dictionary<Type, string> GetScheduledMessages(
        IModuleRegister modules, Dictionary<string, string> schedules)
    {
        var messages = modules
            .Where(module => module is IScheduledMessagesModule)
            .Select(module => module as IScheduledMessagesModule)
            .SelectMany(module => module.GetScheduledMessages())
            .ToList();

        Dictionary<Type, string> scheduledMessages = [];
        foreach (var schedulePair in schedules)
        {
            var type = GetTypeByFullName(schedulePair.Key);
            if (type != null && messages.Contains(type))
            {
                scheduledMessages.Add(type, schedulePair.Value);
            }
        }

        return scheduledMessages;
    }

    protected Type? GetTypeByFullName(string fullTypeName)
    => MessageRegister?.GetTypeByName(fullTypeName);
}
