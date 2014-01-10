using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Commands
{
    public class CommandResult
    {
        public int ExecutionResult { get; set; }
        public Dictionary<string, object> OutputValues { get; set; }
    }
}
