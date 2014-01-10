using System;
using System.Collections.Generic;

using N_Dexed.Deployment.Common.Domain.Messaging;

namespace N_Dexed.Deployment.Common.Domain.Commands
{
    public interface ICommandProcessor : IMessagingComponent
    {
        /// <summary>
        /// Executes a single IExecutableCommand and returns the return code
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResult ExecuteCommand(CommandInfo command);

        /// <summary>
        /// Executes one or more IExecutableCommands and returns a Dictionary that maps Command Ids to returns codes
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        Dictionary<Guid, CommandResult> ExecuteCommands(List<CommandInfo> commands);
    }
}
