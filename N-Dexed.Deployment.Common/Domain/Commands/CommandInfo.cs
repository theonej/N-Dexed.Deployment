using System;
using System.Collections.Generic;

namespace N_Dexed.Deployment.Common.Domain
{
    /// <summary>
    /// This class is used as a message to send instructions to the command processor about IExecutableCommands
    /// </summary>
    public class CommandInfo : IItemInfo
    {
        public Guid Id { get; set; }
        public Guid CommandLibraryId { get; set; }
        public Guid SystemId { get; set; }
        public Guid CustomerId { get; set; }
        public string QualifiedCommandTypeName { get; set; }
        public string MethodName { get; set; }
        public List<CommandArgument> Arguments { get; set; }
    }
}
