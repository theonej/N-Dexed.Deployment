using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;

namespace N_Dexed.Deployment.Common.Domain.Systems
{
    public class SystemInfo : IItemInfo
    {
        public Guid Id { get; set; }
        public SystemProviders Provider { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public AccessCredentials Credentials { get; set; }

        public void Create(CreateSystemInstruction instruction)
        {
            if (instruction.Id == Guid.Empty)
                instruction.Id = Guid.NewGuid();

            this.Id = instruction.Id;
            this.SystemName = instruction.SystemName;
            this.Description = instruction.Description;
            this.Provider = (SystemProviders)instruction.ProviderId;
            this.CustomerId = instruction.CustomerId;
            this.CreatedDateTime = DateTime.Now;

            this.Credentials = new AccessCredentials();
            this.Credentials.AccessKey = instruction.AccessKey;
            this.Credentials.SecretKey = instruction.SecretKey;
        }
    }
}
