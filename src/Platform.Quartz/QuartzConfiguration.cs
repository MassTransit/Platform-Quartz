namespace Platform.QuartzService
{
    using System;
    using System.Collections.Specialized;
    using MassTransit.Transports.InMemory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    public class QuartzConfiguration
    {
        readonly IOptions<QuartzOptions> _options;
        readonly ILogger<QuartzConfiguration> _logger;

        public QuartzConfiguration(IOptions<QuartzOptions> options, IOptions<OtherOptions> otherOptions, ILogger<QuartzConfiguration> logger)
        {
            _options = options;
            _logger = logger;

            var queueName = otherOptions.Value.Scheduler ?? options.Value.Queue;
            if (string.IsNullOrWhiteSpace(queueName))
                queueName = "quartz";
            else
            {
                if (Uri.IsWellFormedUriString(queueName, UriKind.Absolute))
                    queueName = new Uri(queueName).GetQueueOrExchangeName();
            }

            Queue = queueName;
        }

        public string Queue { get; }

        public int ConcurrentMessageLimit => _options.Value.ConcurrentMessageLimit ?? 16;

        public NameValueCollection Configuration
        {
            get
            {
                var configuration = new NameValueCollection(13)
                {
                    {"quartz.scheduler.instanceName", _options.Value.InstanceName ?? "MassTransit-Scheduler"},
                    {"quartz.scheduler.instanceId", "AUTO"},
                    {"quartz.plugin.timeZoneConverter.type","Quartz.Plugin.TimeZoneConverter.TimeZoneConverterPlugin, Quartz.Plugins.TimeZoneConverter"},
                    {"quartz.serializer.type", "json"},
                    {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
                    {"quartz.threadPool.threadCount", (_options.Value.ThreadCount ?? 10).ToString("F0")},
                    {"quartz.jobStore.misfireThreshold", "60000"},
                    {"quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                    {"quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz"},
                    {"quartz.jobStore.tablePrefix", _options.Value.TablePrefix ?? "QRTZ_"},
                    {"quartz.jobStore.dataSource", "default"},
                    {"quartz.jobStore.clustered", $"{_options.Value.Clustered ?? true}"},
                    {"quartz.dataSource.default.provider", _options.Value.Provider ?? "SqlServer"},
                    {
                        "quartz.dataSource.default.connectionString", _options.Value.ConnectionString ??
                        "Server=tcp:localhost;Database=quartznet;Persist Security Info=False;User ID=sa;Password=Quartz!DockerP4ss;Encrypt=False;TrustServerCertificate=True;"
                    },
                    {"quartz.jobStore.useProperties", "true"}
                };

                foreach (var key in configuration.AllKeys)
                {
                    _logger.LogInformation("{Key} = {Value}", key, configuration[key]);
                }

                return configuration;
            }
        }
    }
}