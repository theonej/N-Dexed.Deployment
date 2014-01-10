using System;
using System.Collections.Generic;
using System.Reflection;
using N_Dexed.Deployment.Common.Domain.Commands;

namespace N_Dexed.Deployment.AWS
{
    internal static class ProcessingRegistry
    {
        private static Dictionary<Guid, Assembly> AssemblyRegistry;
        private static Dictionary<Guid, IExecutableCommand> CommandRegistry;

        internal static Assembly GetAssembly(Guid commandLibraryId)
        {
            Assembly returnValue = null;

            if (AssemblyRegistry == null)
            {
                AssemblyRegistry = new Dictionary<Guid, Assembly>();
            }

            if (AssemblyRegistry.ContainsKey(commandLibraryId))
            {
                returnValue = AssemblyRegistry[commandLibraryId];
            }

            return returnValue;
        }

        internal static void AddAssembly(Guid commandLibraryId, Assembly assembly)
        {
            if (AssemblyRegistry == null)
            {
                AssemblyRegistry = new Dictionary<Guid, Assembly>();
            }

            if (!AssemblyRegistry.ContainsKey(commandLibraryId))
            {
                AssemblyRegistry.Add(commandLibraryId, assembly);
            }
        }

        internal static IExecutableCommand GetCommand(Guid commandId)
        {
            IExecutableCommand returnValue = null;

            if (CommandRegistry == null)
            {
                CommandRegistry = new Dictionary<Guid, IExecutableCommand>();
            }

            if (CommandRegistry.ContainsKey(commandId))
            {
                returnValue = CommandRegistry[commandId];
            }

            return returnValue;
        }

        internal static void AddCommand(Guid commandId, IExecutableCommand command)
        {
            if (CommandRegistry == null)
            {
                CommandRegistry = new Dictionary<Guid, IExecutableCommand>();
            }

            if (!CommandRegistry.ContainsKey(commandId))
            {
                CommandRegistry.Add(commandId, command);
            }
        }
    }
}
