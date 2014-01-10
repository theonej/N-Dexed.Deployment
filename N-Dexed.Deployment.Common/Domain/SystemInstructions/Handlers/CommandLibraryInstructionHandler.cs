using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.SystemInstructions.Handlers
{
    public class CommandLibraryInstructionHandler : ISystemInstructionHandler<CreateCommandLibraryInstruction>
    {
        private readonly IRepository<CommandLibraryInfo> m_LibraryRepository;
        private readonly IMessageLogger m_Logger;

        public CommandLibraryInstructionHandler(IRepository<CommandLibraryInfo> libraryRepository,
                                                IMessageLogger logger)
        {
            Condition.Requires(libraryRepository).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            m_LibraryRepository = libraryRepository;
            m_Logger = logger;
        }

        public void Handle(CreateCommandLibraryInstruction instruction)
        {
            if (instruction.Id == Guid.Empty)
            {
                instruction.Id = Guid.NewGuid();
            }

            CommandLibraryInfo commandLibrary = new CommandLibraryInfo();
            commandLibrary.CreatedDateTime = DateTime.Now;
            commandLibrary.CustomerId = instruction.CustomerId;
            commandLibrary.Id = instruction.Id;
            commandLibrary.LibraryName = instruction.LibraryName;
            commandLibrary.LibraryUri = instruction.LibraryUri;

            m_LibraryRepository.Save(commandLibrary);
        }
    }
}
