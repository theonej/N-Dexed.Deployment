using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Messaging
{
    public interface IMessageLogger
    {
        void WriteMessage(MessageInfo message);
        void WriteError(ErrorInfo error);
        void WriteException(Exception exception);
    }
}
