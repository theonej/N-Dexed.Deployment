using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Security.Encryptors;

namespace N_Dexed.Deployment.Tests.AWS.Repositories
{
    [TestClass]
    public class DynamoSystemRepository_Test
    {
        [TestMethod]
        public void CreateNewSystemThenGetThenDelete()
        {
            IMessageLogger logger = new DynamoMessageLogger();
            IEncryptor encryptor = new RijndaelManagedEncryptor();
            IRepository<SystemInfo> repository = new DynamoSystemRepository(logger, encryptor);

            SystemInfo system = new SystemInfo();
            system.CreatedDateTime = DateTime.Now;
            system.CustomerId = Guid.NewGuid();
            system.Credentials = new AccessCredentials();
            system.Description = "A Test System";
            system.Id = Guid.NewGuid();
            system.Provider = SystemProviders.AWS;
            system.SystemName = "Test System";

            try
            {
                repository.Save(system);

                SystemInfo item = new SystemInfo();
                item.Id = system.Id;
                item.CustomerId = system.CustomerId;

                SystemInfo foundItem = repository.Get(item);
                Assert.IsNotNull(foundItem);
                Assert.AreEqual(foundItem.CreatedDateTime, system.CreatedDateTime);
                Assert.AreEqual(foundItem.CustomerId, system.CustomerId);
                Assert.AreEqual(foundItem.Description, system.Description);
                Assert.AreEqual(foundItem.Id, system.Id);
                Assert.AreEqual(foundItem.Provider, system.Provider);
                Assert.AreEqual(foundItem.SystemName, system.SystemName);
            }
            finally
            {
                repository.Delete(system);
            }
        }

        [TestMethod]
        public void CreateNewSystemThenGetByCustomerIdThenDelete()
        {
            IMessageLogger logger = new DynamoMessageLogger();
            IEncryptor encryptor = new RijndaelManagedEncryptor();
            IRepository<SystemInfo> repository = new DynamoSystemRepository(logger, encryptor);

            SystemInfo system = new SystemInfo();
            system.CreatedDateTime = DateTime.Now;
            system.CustomerId = Guid.NewGuid();
            system.Credentials = new AccessCredentials();
            system.Description = "A Test System";
            system.Id = Guid.NewGuid();
            system.Provider = SystemProviders.AWS;
            system.SystemName = "Test System";

            try
            {
                repository.Save(system);

                SystemInfo searchCriteria = new SystemInfo();
                searchCriteria.CustomerId = system.CustomerId;

                SystemInfo foundItem = repository.Search(searchCriteria).FirstOrDefault();
                Assert.IsNotNull(foundItem);
                Assert.AreEqual(foundItem.CreatedDateTime, system.CreatedDateTime);
                Assert.AreEqual(foundItem.CustomerId, system.CustomerId);
                Assert.AreEqual(foundItem.Description, system.Description);
                Assert.AreEqual(foundItem.Id, system.Id);
                Assert.AreEqual(foundItem.Provider, system.Provider);
                Assert.AreEqual(foundItem.SystemName, system.SystemName);
            }
            finally
            {
                repository.Delete(system);
            }
        }
    }
}
