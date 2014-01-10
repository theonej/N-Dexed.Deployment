using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.Common.Domain.Systems;

namespace N_Dexed.Deployment.AWS.SystemInstructions.Handlers
{
    public class AwsSystemsInstructionHandler : ISystemInstructionHandler<CreateSystemInstruction>
    {
        private readonly IRepository<SystemInfo> m_SystemsRepository;

        public AwsSystemsInstructionHandler(IRepository<SystemInfo> systemsRepository)
        {
            Condition.Requires(systemsRepository).IsNotNull();

            m_SystemsRepository = systemsRepository;
        }

        public void Handle(CreateSystemInstruction instruction)
        {
            SystemInfo system = new SystemInfo();
            system.Create(instruction);

            m_SystemsRepository.Save(system);
        }
    }
}
