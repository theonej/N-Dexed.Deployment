using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Customer
{
    public class UserInfo : IItemInfo
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string[] UserPermissions { get; set; }
    }
}
