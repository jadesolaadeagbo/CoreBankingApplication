using CoreBanking.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.DataAccessLayer.ServiceBus.Handlers
{
    public class TransactionEventServiceBusHandler : BaseMessageHandler<MoneyTransferredEvent>
    {
        public TransactionEventServiceBusHandler(
            IServiceBusClientFactory clientFactory,
            ServiceBusConfiguration config,
            ILogger<TransactionEventServiceBusHandler> logger,
            IMediator mediator)
            : base(clientFactory, config.TransactionTopicName, "fraud-detection", logger, mediator)
        {
        }

    }
}
