using CoreBanking.Core.Entities;
using CoreBanking.Core.Models;
using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account> GetByIdAsync(AccountId accountId);
        Task<Account> GetByAccountNumberAsync(AccountNumber accountNumber);
        Task<IEnumerable<Account>> GetByCustomerIdAsync(CustomerId customerId);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task<bool> AccountNumberExistsAsync(AccountNumber accountNumber);
    }
}
