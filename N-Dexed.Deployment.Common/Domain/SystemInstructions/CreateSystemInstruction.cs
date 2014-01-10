using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.SystemInstructions
{
    public class CreateSystemInstruction : ISystemInstruction
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public int ProviderId { get; set; }
        public Guid CustomerId { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
