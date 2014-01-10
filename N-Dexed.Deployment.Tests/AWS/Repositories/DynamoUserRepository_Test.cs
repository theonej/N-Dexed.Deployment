using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Security.Hashers;

namespace N_Dexed.Deployment.Tests.AWS.Repositories
{
    [TestClass]
    public class DynamoUserRepository_Test
    {
        [TestMethod]
        public void CreateNewUserThenDelete()
        {
            DynamoUserRepository repository = new DynamoUserRepository(new DynamoMessageLogger());

            UserInfo user = new UserInfo();
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.PasswordHash = "A TRULY HASHED PASSWORD";
            user.UserName = "Test User";
            user.CreatedDateTime = DateTime.Now;
            user.UserPermissions = new string[1]{
                                                    "Administrator"
                                                };
            try
            {
                repository.Save(user);
            }
            finally
            {
                repository.Delete(user);
            }
        }

        [TestMethod]
        public void CreateNewUserThenGetThenDelete()
        {
            DynamoUserRepository repository = new DynamoUserRepository(new DynamoMessageLogger());

            UserInfo user = new UserInfo();
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.PasswordHash = "A TRULY HASHED PASSWORD";
            user.UserName = "Test User";
            user.CreatedDateTime = DateTime.Now;

            try
            {
                repository.Save(user);

                UserInfo userInfo = repository.Get(user);
                Assert.IsNotNull(userInfo);
                Assert.AreEqual(user.CustomerId, userInfo.CustomerId);
                Assert.AreEqual(user.EmailAddress, userInfo.EmailAddress);
                Assert.AreEqual(user.Id, userInfo.Id);
                Assert.AreEqual(user.PasswordHash, userInfo.PasswordHash);
                Assert.AreEqual(user.UserName, userInfo.UserName);
            }
            finally
            {
                repository.Delete(user);
            }
        }

        [TestMethod]
        public void CreateNewUserThenFindByUserNameThenDelete()
        {
            DynamoUserRepository repository = new DynamoUserRepository(new DynamoMessageLogger());

            UserInfo user = new UserInfo();
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.PasswordHash = "A TRULY HASHED PASSWORD";
            user.UserName = "Test User";
            user.CreatedDateTime = DateTime.Now;

            try
            {
                repository.Save(user);

                UserInfo searchCriteria = new UserInfo();
                searchCriteria.UserName = user.UserName;

                List<UserInfo> users = repository.Search(searchCriteria);

                Assert.AreNotEqual(0, users.Count);
                UserInfo userInfo = users[0];

                Assert.IsNotNull(userInfo);
                Assert.AreEqual(user.CustomerId, userInfo.CustomerId);
                Assert.AreEqual(user.EmailAddress, userInfo.EmailAddress);
                Assert.AreEqual(user.Id, userInfo.Id);
                Assert.AreEqual(user.PasswordHash, userInfo.PasswordHash);
                Assert.AreEqual(user.UserName, userInfo.UserName);
            }
            finally
            {
                repository.Delete(user);
            }
        }

        [TestMethod]
        public void CreateNewUserThenFindByEmailThenDelete()
        {
            DynamoUserRepository repository = new DynamoUserRepository(new DynamoMessageLogger());

            UserInfo user = new UserInfo();
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.PasswordHash = "A TRULY HASHED PASSWORD";
            user.UserName = "Test User";
            user.CreatedDateTime = DateTime.Now;

            try
            {
                repository.Save(user);

                UserInfo searchCriteria = new UserInfo();
                searchCriteria.EmailAddress = user.EmailAddress;

                List<UserInfo> users = repository.Search(searchCriteria);

                Assert.AreNotEqual(0, users.Count);
                UserInfo userInfo = users[0];

                Assert.IsNotNull(userInfo);
                Assert.AreEqual(user.CustomerId, userInfo.CustomerId);
                Assert.AreEqual(user.EmailAddress, userInfo.EmailAddress);
                Assert.AreEqual(user.Id, userInfo.Id);
                Assert.AreEqual(user.PasswordHash, userInfo.PasswordHash);
                Assert.AreEqual(user.UserName, userInfo.UserName);
            }
            finally
            {
                repository.Delete(user);
            }
        }

        [TestMethod]
        public void CreateNewUserThenFindByIdThenDelete()
        {
            DynamoUserRepository repository = new DynamoUserRepository(new DynamoMessageLogger());

            UserInfo user = new UserInfo();
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.PasswordHash = "A TRULY HASHED PASSWORD";
            user.UserName = "Test User";
            user.CreatedDateTime = DateTime.Now;

            try
            {
                repository.Save(user);

                UserInfo searchCriteria = new UserInfo();
                searchCriteria.Id = user.Id;

                List<UserInfo> users = repository.Search(searchCriteria);

                Assert.AreNotEqual(0, users.Count);
                UserInfo userInfo = users[0];

                Assert.IsNotNull(userInfo);
                Assert.AreEqual(user.CustomerId, userInfo.CustomerId);
                Assert.AreEqual(user.EmailAddress, userInfo.EmailAddress);
                Assert.AreEqual(user.Id, userInfo.Id);
                Assert.AreEqual(user.PasswordHash, userInfo.PasswordHash);
                Assert.AreEqual(user.UserName, userInfo.UserName);
            }
            finally
            {
                repository.Delete(user);
            }
        }

        [TestMethod]
        public void CreateNewUserThenFindByUserNameThenAuthenticateThenDelete()
        {
            string password = "A TRULY HASHED PASSWORD";

            PublicPrivateKeyHasher hasher = new PublicPrivateKeyHasher();
            string publicKey = ConfigurationManager.AppSettings["PublicKey"];
            string privateKey = ConfigurationManager.AppSettings["PrivateKey"];
            hasher.RegisterKeyPair(publicKey, privateKey);

            DynamoUserRepository repository = new DynamoUserRepository(new DynamoMessageLogger());

            UserInfo user = new UserInfo();
            user.CustomerId = Guid.NewGuid();
            user.EmailAddress = "test.user@example.com";
            user.Id = Guid.NewGuid();
            user.UserName = "Test User";
            user.CreatedDateTime = DateTime.Now;
            user.PasswordHash = hasher.GenerateHash(password, user.CreatedDateTime.ToString(), publicKey);

            try
            {
                repository.Save(user);

                UserInfo searchCriteria = new UserInfo();
                searchCriteria.UserName = user.UserName;

                List<UserInfo> users = repository.Search(searchCriteria);

                Assert.AreNotEqual(0, users.Count);
                UserInfo userInfo = users[0];

                string hashedPassword = hasher.GenerateHash(password, userInfo.CreatedDateTime.ToString(), publicKey);
                Assert.AreEqual(hashedPassword, userInfo.PasswordHash);
            }
            finally
            {
                repository.Delete(user);
            }
        }
    }
}
