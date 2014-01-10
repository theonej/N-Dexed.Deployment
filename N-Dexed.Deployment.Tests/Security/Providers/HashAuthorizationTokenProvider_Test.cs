using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Security.Encryptors;
using N_Dexed.Deployment.Security.Hashers;
using N_Dexed.Deployment.Security.Providers;

namespace N_Dexed.Deployment.Tests.Security.Providers
{
    [TestClass]
    public class HashAuthorizationTokenProvider_Test
    {
        [TestMethod]
        public void GenerateUserHashTokenThenValidate()
        {
            IHashProvider hashProvider = new PublicPrivateKeyHasher();
            IEncryptor encryptor = new RijndaelManagedEncryptor();
            string publicKey = ConfigurationManager.AppSettings["PublicKey"];
            IAuthorizationTokenProvider tokenProvider = new HashAuthorizationTokenProvider(hashProvider, encryptor);

            UserInfo user = new UserInfo();

            user.CreatedDateTime = DateTime.Now;
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.PasswordHash = hashProvider.GenerateHash("password", user.CreatedDateTime.ToString(), publicKey);
            user.UserName = "test.user";

            string token = tokenProvider.GenerateAuthorizationToken(user.Id);

            Guid userId = tokenProvider.ValidateAuthorizationToken(token);
            Assert.AreEqual(userId, user.Id);
        }
    }
}
