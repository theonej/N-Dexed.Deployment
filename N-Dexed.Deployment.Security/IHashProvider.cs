using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Security
{
    public interface IHashProvider
    {
        string GenerateHash(string value, string salt, string key);
    }
}
