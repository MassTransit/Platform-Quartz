namespace Platform.QuartzService
{
    using GreenPipes.Partitioning;
    using MassTransit;
    using MassTransit.QuartzIntegration;


    public class QuartzEndpointDefinition :
        IEndpointDefinition<ScheduleMessageConsumer>,
        IEndpointDefinition<CancelScheduledMessageConsumer>
    {
        readonly QuartzConfiguration _configuration;

        public QuartzEndpointDefinition(QuartzConfiguration configuration)
        {
            _configuration = configuration;

            Partition = new Partitioner(_configuration.ConcurrentMessageLimit, new Murmur3UnsafeHashGenerator());
        }

        public IPartitioner Partition { get; }

        public virtual bool ConfigureConsumeTopology => true;

        public virtual bool IsTemporary => false;

        public virtual int? PrefetchCount => _configuration.ConcurrentMessageLimit;

        public virtual int? ConcurrentMessageLimit => _configuration.ConcurrentMessageLimit;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _configuration.Queue;
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}