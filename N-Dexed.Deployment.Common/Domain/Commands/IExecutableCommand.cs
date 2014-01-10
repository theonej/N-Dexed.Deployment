using System;
using System.Collections.Generic;

using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;

namespace N_Dexed.Deployment.Common.Domain.Commands
{
    public interface IExecutableCommand : IMessagingComponent
    {
        #region Properties

        AccessCredentials CommandCredentials { get; set; }
        
        #endregion

        #region Methods

        /// <summary>
        /// This method should return a 0 if it is successfull, and a 1 if it fails.
        /// Error messages should be stored in the Errors list
        /// Exceptions can be used for flow control, but the Exceptions should be persisted to the Error list before being thrown
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        CommandResult Execute(List<CommandArgument> arguments);

        /// <summary>
        /// This method should return a list of arguments in the order they should be passed to the Execute method
        /// </summary>
        /// <returns></returns>
        List<CommandArgument> GetOrderedArgumentLists();

        #endregion
    }
}
