using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.Models
{
    public record SimulatedCustomerProfile(
        int BaseScore,
        decimal TotalDebt,
        int ActiveAccounts,
        int LatePayments,
        decimal CreditUtilization,
        int OldestAccountAgeMonths,
        string[] CreditFactors
    );
}
