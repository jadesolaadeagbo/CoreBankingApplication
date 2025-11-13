using CoreBanking.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.DataAccessLayer.ServiceBus.Handlers
{
    public class CustomerEventServiceBusHandler : BaseMessageHandler<CustomerCreatedEvent>
    {
        public CustomerEventServiceBusHandler(
            IServiceBusClientFactory clientFactory,
            ServiceBusConfiguration config,
            ILogger<CustomerEventServiceBusHandler> logger,
            IMediator mediator)
            : base(clientFactory, config.CustomerTopicName, "notifications", logger, mediator)
        {
        }
    }
}
