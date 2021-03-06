namespace Platform.QuartzService
{
    using GreenPipes;
    using MassTransit;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Definition;
    using MassTransit.QuartzIntegration;
    using MassTransit.Scheduling;


    public class CancelScheduledMessageConsumerDefinition :
        ConsumerDefinition<CancelScheduledMessageConsumer>
    {
        readonly QuartzEndpointDefinition _endpointDefinition;

        public CancelScheduledMessageConsumerDefinition(QuartzEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CancelScheduledMessageConsumer> consumerConfigurator)
        {
            consumerConfigurator.Message<CancelScheduledMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.TokenId));
        }
    }
}