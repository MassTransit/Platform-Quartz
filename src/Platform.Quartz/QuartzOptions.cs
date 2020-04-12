using System;
using System.Collections.Specialized;

namespace Platform.Quartz
{
    public class QuartzSettings
    {
        public string Name { get; set; }
    }

    public class QuartzOptions
    {
        public string QueueName
        {
            get
            {
                var queueName = Environment.GetEnvironmentVariable("MT_Quartz");
                if (string.IsNullOrWhiteSpace(queueName))
                    queueName = "quartz";

                return queueName;
            }
        }

        public string ConnectionString
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable("MT_Quartz_ConnectionString");
                if (string.IsNullOrWhiteSpace(connectionString))
                    connectionString =
                        "Server=tcp:localhost;Database=quartznet;Persist Security Info=False;User ID=sa;Password=Quartz!DockerP4ss;Encrypt=False;TrustServerCertificate=True;";

                return connectionString;
            }
        }

        public int ConcurrentMessageLimit
        {
            get
            {
                var messageLimit = Environment.GetEnvironmentVariable("MT_Quartz_ConcurrentMessageLimit");
                if (string.IsNullOrWhiteSpace(messageLimit) || !int.TryParse(messageLimit, out var limit) || limit < 1)
                    return 16;

                return limit;
            }
        }

        public NameValueCollection Configuration
        {
            get
            {
                var collection = new NameValueCollection(10)
                {
                    {"quartz.scheduler.instanceName", "MassTransit-Scheduler"},
                    {"quartz.scheduler.instanceId", "AUTO"},
                    {"quartz.serializer.type", "json"},
                    {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
                    {"quartz.threadPool.threadCount", "10"},
                    {"quartz.jobStore.misfireThreshold", "60000"},
                    {"quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                    {"quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz"},
                    {"quartz.jobStore.tablePrefix", "QRTZ_"},
                    {"quartz.jobStore.dataSource", "quartzDS"},
                    {"quartz.dataSource.quartzDS.provider", "SqlServer"},
                    {"quartz.dataSource.quartzDS.connectionString", ConnectionString},
                    {"quartz.jobStore.useProperties", "true"}
                };

                return collection;
            }
        }
    }
}