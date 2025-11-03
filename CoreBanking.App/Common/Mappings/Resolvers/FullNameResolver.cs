using AutoMapper;
using CoreBanking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Common.Mappings.Resolvers
{
    public class FullNameResolver : IValueResolver<Customer, object, string>
    {
        public string Resolve(Customer source, object destination, string destMember, ResolutionContext context)
            => $"{source.FirstName} {source.LastName}";
    }
}
