using GreenPipes;
using GreenPipes.Partitioning;
using MassTransit;
using MassTransit.QuartzIntegration;

namespace Platform.Quartz
{
    public class QuartzEndpointDefinition :
        IEndpointDefinition<ScheduleMessageConsumer>,
        IEndpointDefinition<CancelScheduledMessageConsumer>
    {
        readonly QuartzOptions _options;

        public QuartzEndpointDefinition(QuartzOptions options)
        {
            _options = options;
        }

        public IPartitioner Partition { get; private set; }

        public virtual bool ConfigureConsumeTopology => true;

        public virtual bool IsTemporary => false;

        public virtual int? PrefetchCount => default;

        public virtual int? ConcurrentMessageLimit => _options.ConcurrentMessageLimit;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _options.QueueName;
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            var partitionCount = _options.ConcurrentMessageLimit;

            Partition = configurator.CreatePartitioner(partitionCount);
        }
    }
}