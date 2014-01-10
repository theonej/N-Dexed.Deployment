using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Messaging
{
    public interface IMessagingComponent
    {
        /// <summary>
        /// Provides a means of exposing error information that should not change process flow
        /// </summary>
        List<ErrorInfo> Errors { get; set; }

        /// <summary>
        /// Provides a means of messaging the system about processes internal to the command execution
        /// </summary>
        List<MessageInfo> Messages { get; set; }
    }
}
