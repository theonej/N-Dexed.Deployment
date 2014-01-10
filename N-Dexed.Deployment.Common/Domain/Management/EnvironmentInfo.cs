using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Management
{
    public class EnvironmentInfo : IItemInfo
    {
        public Guid Id { get; set; }
        public string EnvironmentName { get; set; }
        public string Description { get; set; }
        public string DnsName { get; set; }
        public string EndpointURL { get; set; }
        public List<string> DNSPointerRecords { get; set; }
        public string Status { get; set; }
        public string Health { get; set; }
        public string Version { get; set; }
    }
}
