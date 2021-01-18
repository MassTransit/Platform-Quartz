namespace Platform.QuartzService
{
    using System;
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.Platform.Abstractions;
    using MassTransit.QuartzIntegration;
    using MassTransit.QuartzIntegration.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
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

        public void ConfigureMassTransit(IServiceCollectionBusConfigurator configurator, IServiceCollection services)
        {
            _logger.LogInformation("MassTransit Platform, Quartz Service - Configuring MassTransit");

            services.Configure<OtherOptions>(_configuration);
            services.Configure<QuartzOptions>(_configuration.GetSection("Quartz"));
            services.AddSingleton<QuartzConfiguration>();

            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<QuartzConfiguration>();

                return new InMemorySchedulerOptions
                {
                    SchedulerFactory = new StdSchedulerFactory(options.Configuration),
                    QueueName = options.Queue
                };
            });
            services.AddSingleton<SchedulerBusObserver>();
            services.AddSingleton(provider => provider.GetRequiredService<SchedulerBusObserver>().Scheduler);

            services.AddSingleton<QuartzEndpointDefinition>();

            configurator.AddConsumer<ScheduleMessageConsumer>(typeof(ScheduleMessageConsumerDefinition));
            configurator.AddConsumer<CancelScheduledMessageConsumer>(typeof(CancelScheduledMessageConsumerDefinition));
        }

        public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IBusRegistrationContext context)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var options = context.GetRequiredService<QuartzConfiguration>();

            var schedulerAddress = new Uri($"queue:{options.Queue}");

            _logger.LogInformation("Configuring Quartz: {SchedulerAddress}", schedulerAddress);

            configurator.ConnectBusObserver(context.GetRequiredService<SchedulerBusObserver>());
        }
    }
}