using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.Common.Domain;
using N_Dexed.Deployment.Common.Domain.Repositories;

namespace N_Dexed.Deployment.Tests.AWS
{
    [TestClass]
    public class DynamoCommandLibraryRepository_Test
    {
        [TestMethod]
        public void CreateNewCommandLibraryThenDelete()
        {
            CommandLibraryInfo commandLibrary = new CommandLibraryInfo();
            commandLibrary.Id = Guid.NewGuid();
            commandLibrary.CustomerId = Guid.NewGuid();
            commandLibrary.LibraryName = "TestLibrary";
            commandLibrary.LibraryUri = string.Format("https://n-dexed.deployment.command.libraries.s3-website-us-east-1.amazonaws.com/{0}/{1}/{2}.dll", commandLibrary.CustomerId, commandLibrary.Id, commandLibrary.LibraryName);
            commandLibrary.CreatedDateTime = DateTime.Now;

            IRepository<CommandLibraryInfo> repository = new DynamoCommandLibraryRepository();
            try
            {
                repository.Save(commandLibrary);
            }
            finally
            {
                repository.Delete(commandLibrary);
            }
        }

        [TestMethod]
        public void CreateCommandLibraryThenGetThenDelete()
        {
            CommandLibraryInfo commandLibrary = new CommandLibraryInfo();
            commandLibrary.Id = Guid.NewGuid();
            commandLibrary.CustomerId = Guid.NewGuid();
            commandLibrary.LibraryName = "TestLibrary";
            commandLibrary.LibraryUri = string.Format("https://n-dexed.deployment.command.libraries.s3-website-us-east-1.amazonaws.com/{0}/{1}/{2}.dll", commandLibrary.CustomerId, commandLibrary.Id, commandLibrary.LibraryName);

            IRepository<CommandLibraryInfo> repository = new DynamoCommandLibraryRepository();

            try
            {
                repository.Save(commandLibrary);

                commandLibrary = repository.Get(commandLibrary);
                Assert.IsNotNull(commandLibrary);
            }
            finally
            {
                repository.Delete(commandLibrary);
            }
        }

        [TestMethod]
        public void CreateCommandLibraryThenSearchByCustomerIdThenDelete()
        {
            CommandLibraryInfo commandLibrary = new CommandLibraryInfo();
            commandLibrary.Id = Guid.NewGuid();
            commandLibrary.CustomerId = Guid.NewGuid();
            commandLibrary.LibraryName = "TestLibrary";
            commandLibrary.LibraryUri = string.Format("https://n-dexed.deployment.command.libraries.s3-website-us-east-1.amazonaws.com/{0}/{1}/{2}.dll", commandLibrary.CustomerId, commandLibrary.Id, commandLibrary.LibraryName);

            IRepository<CommandLibraryInfo> repository = new DynamoCommandLibraryRepository();

            try
            {
                repository.Save(commandLibrary);

                CommandLibraryInfo searchCriteria = new CommandLibraryInfo();
                searchCriteria.CustomerId = commandLibrary.CustomerId;

                List<CommandLibraryInfo> searchResults = repository.Search(searchCriteria);
                Assert.IsNotNull(searchResults);
                Assert.AreEqual(1, searchResults.Count);
            }
            finally
            {
                repository.Delete(commandLibrary);
            }
        }
    }
}
