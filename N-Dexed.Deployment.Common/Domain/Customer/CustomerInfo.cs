using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Customer
{
    public class CustomerInfo : IItemInfo
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public AccessCredentials Credentials { get; set; }
        public string AdminEmailAddress { get; set; }
    }
}
