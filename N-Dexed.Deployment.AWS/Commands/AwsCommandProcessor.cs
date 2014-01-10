using System;
using System.Collections.Generic;
using System.Reflection;
using CuttingEdge.Conditions;

using N_Dexed.Deployment.AWS;
using N_Dexed.Deployment.Common.Domain;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Commands;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Common.Domain.Systems;

namespace N_Dexed.Deployment.AWS.Commands
{
    public class AwsCommandProcessor : ICommandProcessor
    {
        private const int FAILURE_RETURN_CODE = 0;

        public List<ErrorInfo> Errors { get; set; }
        public List<MessageInfo> Messages { get; set; }

        private readonly IRepository<CommandLibraryInfo> m_CommandLibraryRepository;
        private readonly IRepository<SystemInfo> m_SystemRepository;
        private readonly IMessageLogger m_MessageLogger;

        public AwsCommandProcessor(IRepository<CommandLibraryInfo> commandLibraryRepository,
                                   IRepository<SystemInfo> systemRepository,
                                   IMessageLogger messageLogger)
        {
            Condition.Requires(commandLibraryRepository).IsNotNull();
            Condition.Requires(systemRepository).IsNotNull();
            Condition.Requires(messageLogger).IsNotNull();

            m_CommandLibraryRepository = commandLibraryRepository;
            m_SystemRepository = systemRepository;

            m_MessageLogger = messageLogger;
        }

        public CommandResult ExecuteCommand(CommandInfo command)
        {
            //use the command library Id to download the command library
            CommandLibraryInfo commandLibrary = GetCommandLibrary(command.CommandLibraryId, command.CustomerId);

            //get the IExecutableCommand specified by the command name and type
            IExecutableCommand executableCommand = GetExecutableCommand(command, commandLibrary);

            //get the system aws access keys
            SystemInfo system = GetSystem(command.SystemId);

            //assign the system credentials as the credentials of the command
            executableCommand.CommandCredentials = system.Credentials;

            try
            {
                CommandResult returnValue = executableCommand.Execute(command.Arguments);

                return returnValue;
            }
            finally
            {
                Errors = executableCommand.Errors;
                Messages = executableCommand.Messages;

                //get any messages and add them to the message log
                LogMessages();
                LogErrors();
            }
        }

        public Dictionary<Guid, CommandResult> ExecuteCommands(List<CommandInfo> commands)
        {
            Dictionary<Guid, CommandResult> returnValue = new Dictionary<Guid, CommandResult>();

            foreach (CommandInfo command in commands)
            {
                CommandResult returnCode = ExecuteCommand(command);
                returnValue.Add(command.Id, returnCode);
            }

            return returnValue;
        }

        #region Private Methods

        private void LogMessages()
        {
            if (Messages != null)
            {
                foreach (MessageInfo message in Messages)
                {
                    m_MessageLogger.WriteMessage(message);
                }
            }
        }

        private void LogErrors()
        {
            if (Errors != null)
            {
                foreach (ErrorInfo error in Errors)
                {
                    m_MessageLogger.WriteError(error);
                }
            }
        }

        private CommandLibraryInfo GetCommandLibrary(Guid commandLibraryId, Guid customerId)
        {
            CommandLibraryInfo item = new CommandLibraryInfo();
            item.Id = commandLibraryId;
            item.CustomerId = customerId;

            CommandLibraryInfo returnValue = m_CommandLibraryRepository.Get(item);

            return returnValue;
        }

        private SystemInfo GetSystem(Guid systemId)
        {
            SystemInfo item = new SystemInfo();
            item.Id = systemId;

            SystemInfo returnValue = m_SystemRepository.Get(item);

            return returnValue;
        }

        private Assembly LoadCommandLibraryAssembly(CommandLibraryInfo commandLibrary)
        {
            Assembly commandAssembly = ProcessingRegistry.GetAssembly(commandLibrary.Id);

            if (commandAssembly == null)
            {
                commandAssembly = Assembly.LoadFile(commandLibrary.LibraryUri);
            }

            if (commandAssembly == null)
            {
                string errorMessage = string.Format(ErrorMessages.AssemblyNotLoaded, commandLibrary.LibraryUri);
                throw new OperationCanceledException(errorMessage);
            }

            return commandAssembly;
        }

        private IExecutableCommand GetExecutableCommand(CommandInfo command, CommandLibraryInfo commandLibrary)
        {
            IExecutableCommand executableCommand = ProcessingRegistry.GetCommand(command.Id);
            if (executableCommand == null)
            {

                Assembly commandAssembly = LoadCommandLibraryAssembly(commandLibrary);

                Type commandType = commandAssembly.GetType(command.QualifiedCommandTypeName);
                if (commandType == null)
                {
                    string errorMessage = string.Format(ErrorMessages.TypeNotFound, command.QualifiedCommandTypeName);
                    throw new TypeAccessException(errorMessage);
                }

                executableCommand = (IExecutableCommand)Activator.CreateInstance(commandType);
                ProcessingRegistry.AddCommand(command.Id, executableCommand);
            }

            return executableCommand;
        }

        #endregion
    }
}
