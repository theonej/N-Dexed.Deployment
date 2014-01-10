using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Security.Hashers
{
    public class PublicPrivateKeyHasher : IHashProvider
    {
        private readonly Dictionary<string, string> m_KeyPairs;

        public PublicPrivateKeyHasher()
        {
            m_KeyPairs = new Dictionary<string, string>();
        }

        public void RegisterKeyPair(string publicKey, string privateKey)
        {
            m_KeyPairs.Add(publicKey, privateKey);
        }

        public string GenerateHash(string value, string salt, string key)
        {
            string privateKey = GetPrivateKey(key);

            byte[] saltBytes = GetSaltBytes(salt);

            Rfc2898DeriveBytes derivedBytesProvider = new Rfc2898DeriveBytes(value, saltBytes);

            byte[] derivedBytes = derivedBytesProvider.GetBytes(128);

            string hash = Convert.ToBase64String(derivedBytes);

            return hash;
        }

        #region Private Methods

        private static byte[] GetSaltBytes(string privateKey)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(privateKey);

            return saltBytes;
        }

        private string GetPrivateKey(string publicKey)
        {
            string privateKey = null;

            if (m_KeyPairs.ContainsKey(publicKey))
            {
                privateKey = m_KeyPairs[publicKey];
            }

            return privateKey;
        }

        #endregion
    }
}
