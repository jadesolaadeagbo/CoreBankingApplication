using CoreBanking.Core.Enums;
using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Entities
{
    public class Account
    {
        public AccountId AccountId { get; private set; }
        public AccountNumber AccountNumber { get; private set; }
        public AccountType AccountType { get; private set; }
        public Money Balance { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public Customer Customer { get; private set; } //Navigation Key
        public DateTime DateOpened { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Transaction> _transactions = new();
        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public string? DeletedBy { get; private set; }

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();



        private Account() { }

        public Account(AccountNumber accountNumber, AccountType accountType, CustomerId customerId)
        {
            AccountId = AccountId.Create();
            AccountNumber = accountNumber;
            AccountType = accountType;
            CustomerId = CustomerId.Create();
            Balance = new Money(0);
            DateOpened = DateTime.UtcNow;
            IsActive = true;
        }

        public Transaction Deposit(Money amount, string description = "Deposit")
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot deposit to an inactive account.");

            if(amount.Amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");

            Balance += amount;
            var transaction = new Transaction(
                accountId: AccountId, 
                type:TransactionType.Deposit, 
                amount:amount,
                description: description
                );
            _transactions.Add(transaction);
            return transaction;
        }

        public Transaction Withdraw(Money amount, string description = "Withdrawal")
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot withdraw from an inactive account.");
            if (amount.Amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive.");
            if (Balance.Amount < amount.Amount)
                throw new InvalidOperationException("Insufficient funds for withdrawal.");

            if(AccountType == AccountType.Savings && _transactions.Count(t=> t.Type == TransactionType.Withdrawal) >= 6)
                throw new InvalidOperationException("Savings account must maintain a minimum balance of 1000 after withdrawal.");

            Balance -= amount;

            var transaction = new Transaction(
                accountId: AccountId.Create(),
                type: TransactionType.Withdrawal,
                amount: amount,
                description: description
                );
            _transactions.Add(transaction);
            return transaction;
        }

        public void CloseAccount()
        {
            if (Balance.Amount != 0)
                throw new InvalidOperationException("Cannot close account with remaining balance.");
            IsActive = false;
        }

        public void SoftDelete(string deletedBy)
        {
            if (Balance.Amount != 0)
                throw new InvalidOperationException("Cannot close account with non-zero balance");

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }
    }
}
