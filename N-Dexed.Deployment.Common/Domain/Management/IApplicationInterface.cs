using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N_Dexed.Deployment.Common.Domain.Systems;

namespace N_Dexed.Deployment.Common.Domain.Management
{
    public interface IApplicationInterface
    {
        List<ApplicationInfo> GetApplications(Guid systemId);
        List<EnvironmentInfo> GetEnvironments(Guid systemId, string applicationName);
        ApplicationInfo GetApplication(Guid systemId, string applicationName);
    }
}
