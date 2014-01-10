using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.SystemInstructions
{
    public class CreateCommandLibraryInstruction : ISystemInstruction
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string LibraryName { get; set; }
        public string LibraryUri { get; set; }
    }
}
