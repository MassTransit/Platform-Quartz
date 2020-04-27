namespace Platform.QuartzService
{
    using System;
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.Platform.Abstractions;
    using MassTransit.QuartzIntegration;
    using MassTransit.QuartzIntegration.Configuration;
    using MassTransit.Util;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using Quartz.Impl;


    public class QuartzPlatformStartup :
        IPlatformStartup
    {
        readonly IConfiguration _configuration;
        readonly ILogger<QuartzPlatformStartup> _logger;

        public QuartzPlatformStartup(IConfiguration configuration, ILogger<QuartzPlatformStartup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureMassTransit(IServiceCollectionConfigurator configurator, IServiceCollection services)
        {
            _logger.LogInformation("MassTransit Platform, Quartz Service - Configuring MassTransit");

            services.Configure<OtherOptions>(_configuration);
            services.Configure<QuartzOptions>(_configuration.GetSection("Quartz"));
            services.AddSingleton<QuartzConfiguration>();

            services.AddSingleton(x =>
            {
                var quartzConfiguration = x.GetRequiredService<QuartzConfiguration>();

                var schedulerFactory = new StdSchedulerFactory(quartzConfiguration.Configuration);

                var scheduler = TaskUtil.Await(() => schedulerFactory.GetScheduler());

                return scheduler;
            });

            services.AddSingleton<QuartzEndpointDefinition>();

            configurator.AddConsumer<ScheduleMessageConsumer>(typeof(ScheduleMessageConsumerDefinition));
            configurator.AddConsumer<CancelScheduledMessageConsumer>(typeof(CancelScheduledMessageConsumerDefinition));
        }

        public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IServiceProvider provider)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var scheduler = provider.GetRequiredService<IScheduler>();

            var options = provider.GetRequiredService<QuartzConfiguration>();

            var schedulerAddress = new Uri($"queue:{options.Queue}");

            _logger.LogInformation("Configuring Quartz: {SchedulerAddress}", schedulerAddress);

            configurator.ConnectBusObserver(new SchedulerBusObserver(scheduler, schedulerAddress));
        }
    }
}