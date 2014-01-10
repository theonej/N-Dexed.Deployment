using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.AWS.SystemInstructions.Handlers;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Security.Encryptors;

namespace N_Dexed.Deployment.Tests.AWS.SystemInstructions.Handlers
{
    [TestClass]
    public class AwsSystemsInstructionHandler_Test
    {
        [TestMethod]
        public void CreateNewSystemThenGetThenDelete()
        {
            CreateSystemInstruction instruction = new CreateSystemInstruction();
            instruction.AccessKey = "test access key";
            instruction.CustomerId = Guid.NewGuid();
            instruction.Description = "test description";
            instruction.Id = Guid.NewGuid();
            instruction.ProviderId = 0;
            instruction.SecretKey = "test secret key";
            instruction.SystemName = "test system";

            IMessageLogger logger = new DynamoMessageLogger();
            IEncryptor encryptor = new RijndaelManagedEncryptor();
            IRepository<SystemInfo> repository = new DynamoSystemRepository(logger, encryptor);
            AwsSystemsInstructionHandler handler = new AwsSystemsInstructionHandler(repository);

            SystemInfo item = new SystemInfo();
            item.Id = instruction.Id;
            item.CustomerId = instruction.CustomerId;

            try
            {
                handler.Handle(instruction);

                SystemInfo createdSystem = repository.Get(item);
                Assert.IsNotNull(createdSystem);
                Assert.AreEqual(createdSystem.Credentials.AccessKey, instruction.AccessKey);
                Assert.AreEqual(createdSystem.Credentials.SecretKey, instruction.SecretKey);
                Assert.AreEqual(createdSystem.CustomerId, instruction.CustomerId);
                Assert.AreEqual(createdSystem.Description, instruction.Description);
                Assert.AreEqual(createdSystem.Id, instruction.Id);
                Assert.AreEqual((int)createdSystem.Provider, instruction.ProviderId);
                Assert.AreEqual(createdSystem.SystemName, instruction.SystemName);
            }
            finally
            {
                repository.Delete(item);
            }
        }
    }
}
