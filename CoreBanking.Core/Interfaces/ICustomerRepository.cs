using CoreBanking.Core.Entities;
using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(CustomerId customerId);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task<bool> ExistsAsync(CustomerId customerId);
    }
}
