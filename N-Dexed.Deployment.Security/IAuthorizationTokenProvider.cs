using System;

namespace N_Dexed.Deployment.Security
{
    public interface IAuthorizationTokenProvider
    {
       string GenerateAuthorizationToken(Guid userId);
       Guid ValidateAuthorizationToken(string token);
    }
}
