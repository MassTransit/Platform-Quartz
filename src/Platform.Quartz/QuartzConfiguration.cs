namespace Platform.QuartzService
{
    using System;
    using System.Collections.Specialized;
    using MassTransit.Transports.InMemory;
    using Microsoft.Extensions.Options;


    public class QuartzConfiguration
    {
        readonly IOptions<QuartzOptions> _options;

        public QuartzConfiguration(IOptions<QuartzOptions> options, IOptions<OtherOptions> otherOptions)
        {
            _options = options;

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

        public NameValueCollection Configuration =>
            new NameValueCollection(13)
            {
                {"quartz.scheduler.instanceName", _options.Value.InstanceName ?? "MassTransit-Scheduler"},
                {"quartz.scheduler.instanceId", "AUTO"},
                {"quartz.serializer.type", "json"},
                {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
                {"quartz.threadPool.threadCount", (_options.Value.ThreadCount ?? 10).ToString("F0")},
                {"quartz.jobStore.misfireThreshold", "60000"},
                {"quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                {"quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz"},
                {"quartz.jobStore.tablePrefix", _options.Value.TablePrefix ?? "QRTZ_"},
                {"quartz.jobStore.dataSource", "quartzDS"},
                {"quartz.dataSource.quartzDS.provider", _options.Value.Provider ?? "SqlServer"},
                {"quartz.dataSource.quartzDS.connectionString", _options.Value.ConnectionString},
                {"quartz.jobStore.useProperties", "true"}
            };
    }
}