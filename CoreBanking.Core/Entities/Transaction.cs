using CoreBanking.Core.Enums;
using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Entities
{
    public class Transaction
    {
        public TransactionId TransactionId { get; private set; }
        public AccountId AccountId { get; private set; }
        public Account Account { get; private set; }
        public TransactionType Type { get; private set; }
        public Money Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Reference { get; private set; }

        public Transaction(AccountId accountId)
        {
            AccountId = accountId;
        }
        public Transaction(AccountId accountId, TransactionType type, Money amount, string description = "")
        {
            TransactionId = TransactionId.Create();
            AccountId = accountId;
            Type = type;
            Amount = amount;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Timestamp = DateTime.UtcNow;
            Reference = GenerateReference();
        }

        private string GenerateReference()
        {
            return $"{Timestamp:yyyyMMddHHmmss}-{TransactionId.ToString().Substring(0, 8)}";
        }
    }
}
