using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.ValueObjects
{
    public record TransactionId(Guid Value)
    {
        public static TransactionId Create() => new(Guid.NewGuid());
        public static TransactionId Create(Guid value) => new(value);
    }
}


