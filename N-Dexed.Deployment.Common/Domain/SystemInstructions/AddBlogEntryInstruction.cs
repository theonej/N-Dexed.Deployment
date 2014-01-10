using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.SystemInstructions
{
    public class AddBlogEntryInstruction : ISystemInstruction
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<string> Tags { get; set; }
    }
}
