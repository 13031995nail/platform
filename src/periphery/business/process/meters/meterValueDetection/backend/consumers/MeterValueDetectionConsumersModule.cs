namespace Tmobis.Business.Process.Meters.MeterValues.Detection.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль распознавания показаний счётчиков.
/// Обрабатывает запрос <c>DetectMeterValues</c> и возвращает <c>DetectedMeterValues</c>.
/// </summary>
public class MeterValueDetectionConsumersModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<MeterValueDetectionOptions>(
            configuration.GetSection(
                MeterValueDetectionOptions.MeterValueDetection));
        services.AddTransient<
            IMessageConsumer<DetectMeterValues, DetectedMeterValues>,
            DetectMeterValuesConsumer>();
    }
}
