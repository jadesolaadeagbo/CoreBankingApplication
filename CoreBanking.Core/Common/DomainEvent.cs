using CoreBanking.Core.Enums;
using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Common
{
    public abstract record DomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }

    public record AccountCreatedEvent : DomainEvent { 
        public AccountId AccountId { get; }
        public AccountNumber AccountNumber { get; }
        public CustomerId CustomerId { get; }
        public AccountType AccountType { get; }
        public Money InitialDeposit { get; }

        public AccountCreatedEvent(AccountId accountId, AccountNumber accountNumber, CustomerId customerId, AccountType accountType, Money initialDeposit)
        {
            AccountId = accountId;
            AccountNumber = accountNumber;
            CustomerId = customerId;
            AccountType = accountType;
            InitialDeposit = initialDeposit;
        }
    }
}
