namespace Platform.Quartz
{
    using GreenPipes.Partitioning;
    using MassTransit;
    using MassTransit.QuartzIntegration;


    public class QuartzEndpointDefinition :
        IEndpointDefinition<ScheduleMessageConsumer>,
        IEndpointDefinition<CancelScheduledMessageConsumer>
    {
        readonly QuartzOptions _options;

        public QuartzEndpointDefinition(QuartzOptions options)
        {
            _options = options;

            Partition = new Partitioner(options.ConcurrentMessageLimit, new Murmur3UnsafeHashGenerator());
        }

        public IPartitioner Partition { get; }

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
        }
    }
}