using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Management
{
    public class ApplicationInfo : IItemInfo
    {
        public Guid Id { get; set; }
        public Guid SystemId { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        public List<EnvironmentInfo> Environments { get; set; }
        public List<string> ApplicationVersions { get; set; }
    }
}
