using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime DateCreated {get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Account> _accounts = new();
        public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();
        public Customer(string firstName, string lastName, string email, string phoneNumber)
        {
            CustomerId = Guid.NewGuid();
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            DateCreated = DateTime.UtcNow;
            IsActive = true;
        }

        public void UpdateContactInfo(string email, string phoneNumber)
        {
            if(!IsActive)
                throw new InvalidOperationException("Cannot update contact info for inactive customer.");

            Email = email;
            PhoneNumber = phoneNumber;
        }

        public void Deactivate()
        {
            if(_accounts.Any(a => a.Balance.Amount > 0))
            {
                throw new InvalidOperationException("Cannot deactivate customer with active accounts having balance.");
            }
            IsActive = false;
        }

        internal void AddAccount(Account account)
        {
            _accounts.Add(account);
        }
    }
}
