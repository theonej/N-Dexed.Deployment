using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Resources;

namespace N_Dexed.Deployment.Security.Providers
{
    public class HashAuthorizationTokenProvider : IAuthorizationTokenProvider
    {
        private const int EXPIRATION_MINUTES = 60 * 24;//ONE DAY

        private readonly IHashProvider m_HashProvider;
        private readonly IEncryptor m_Encryptor;

        public HashAuthorizationTokenProvider(IHashProvider hashProvider, IEncryptor encryptor)
        {
            Condition.Requires(hashProvider).IsNotNull();
            Condition.Requires(encryptor).IsNotNull();

            m_HashProvider = hashProvider;
            m_Encryptor = encryptor;
        }

        public string GenerateAuthorizationToken(Guid userId)
        {
            string expiration = DateTime.Now.AddMinutes(EXPIRATION_MINUTES).ToString();

            string publicKey = ConfigurationManager.AppSettings["PublicKey"];

            string userIdHash = m_HashProvider.GenerateHash(userId.ToString(), expiration, publicKey);

            string tokenString = string.Format("{0}|{1}|{2}|{3}", userId, expiration, userIdHash, publicKey);

            string token = m_Encryptor.EncryptValue(tokenString);

            return token;
        }

        public Guid ValidateAuthorizationToken(string token)
        {
            //decrypt the token
            string decryptedToken = m_Encryptor.DecryptValue(token);

            //extract the different peices of the token [userid:expiration:hash]
            string[] tokenParts = decryptedToken.Split('|');
            if (tokenParts.Length != 4)
            {
                throw new FormatException(ErrorMessages.MalformedAuthorizationToken);
            }

            //validate the expiration
            DateTime expiration = DateTime.Parse(tokenParts[1]);
            if (DateTime.Now > expiration)
            {
                throw new AuthenticationException(ErrorMessages.ExpiredToken);
            }

            //validate the hash
            string tokenHash = m_HashProvider.GenerateHash(tokenParts[0], tokenParts[1], tokenParts[3]);
            if (tokenHash != tokenParts[2])
            {

                throw new FormatException(ErrorMessages.MalformedAuthorizationToken);
            }

            Guid userId = Guid.Parse(tokenParts[0]);

            return userId;
        }
    }
}
