// CoreBanking.Core/Events/MoneyTransferredEvent.cs
using CoreBanking.Core.Common;
using CoreBanking.Core.Entities;
using CoreBanking.Core.ValueObjects;
using MediatR;

namespace CoreBanking.Core.Events;

public record MoneyTransferredEvent : DomainEvent, INotification
{
    public TransactionId TransactionId { get; }
    public AccountNumber SourceAccountNumber { get; }
    public AccountNumber DestinationAccountNumber { get; }
    public Money Amount { get; }
    public string Reference { get; }
    public DateTime TransferDate { get; }
    public Guid EventId { get; } = Guid.NewGuid();
    public string EventType { get; } = nameof(MoneyTransferredEvent);
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string TransactionType { get; } = "Transfer";


    public MoneyTransferredEvent(TransactionId transactionId, AccountNumber sourceAccountNumber,
        AccountNumber destinationAccountNumber, Money amount, string reference)
    {
        TransactionId = transactionId;
        SourceAccountNumber = sourceAccountNumber;
        DestinationAccountNumber = destinationAccountNumber;
        Amount = amount;
        Reference = reference;
        TransferDate = DateTime.UtcNow;
    }
}
