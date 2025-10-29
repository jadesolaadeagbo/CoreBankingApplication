using CoreBanking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Core.ValueObjects
{
    public record AccountId(Guid Value)
    {
        public static AccountId Create() => new(Guid.NewGuid());
        public static AccountId Create(Guid value) => new(value);
    }
}

