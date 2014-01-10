using System;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using N_Dexed.Deployment.AWS.Management;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.Common.Domain.Management;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Security.Encryptors;

namespace N_Dexed.Deployment.Tests.AWS.Management
{
    [TestClass]
    public class ElasticBeanstalkApplicationInterface_Test
    {
        [TestMethod]
        public void CreateSystemThenGetApplicationsThenDeleteSystem()
        {
            SystemInfo system = new SystemInfo();
            system.CreatedDateTime = DateTime.Now;
            system.Credentials = new Common.Domain.Customer.AccessCredentials();
            system.Credentials.AccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
            system.Credentials.SecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            system.CustomerId = Guid.NewGuid();
            system.Id = Guid.NewGuid();
            system.Provider = SystemProviders.AWS;
            system.SystemName = "test system";

            IMessageLogger logger = new DynamoMessageLogger();
            IEncryptor encryptor = new RijndaelManagedEncryptor();
            IRepository<SystemInfo> repository = new DynamoSystemRepository(logger, encryptor);

            try
            {
                repository.Save(system);

                ElasticBeanstalkApplicationInterface ebInterface = new ElasticBeanstalkApplicationInterface(repository);
                List<ApplicationInfo> applications = ebInterface.GetApplications(system.Id);
            }
            finally
            {
                repository.Delete(system);
            } 
        }

        [TestMethod]
        public void CreateSystemThenGetEnvironmentsThenDeleteSystem()
        {
            SystemInfo system = new SystemInfo();
            system.CreatedDateTime = DateTime.Now;
            system.Credentials = new Common.Domain.Customer.AccessCredentials();
            system.Credentials.AccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
            system.Credentials.SecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            system.CustomerId = Guid.NewGuid();
            system.Id = Guid.NewGuid();
            system.Provider = SystemProviders.AWS;
            system.SystemName = "test system";

            IMessageLogger logger = new DynamoMessageLogger();
            IEncryptor encryptor = new RijndaelManagedEncryptor();
            IRepository<SystemInfo> repository = new DynamoSystemRepository(logger, encryptor);

            try
            {
                repository.Save(system);

                string applicationName = "test application";

                ElasticBeanstalkApplicationInterface ebInterface = new ElasticBeanstalkApplicationInterface(repository);
                List<EnvironmentInfo> environments = ebInterface.GetEnvironments(system.Id, applicationName);
            }
            finally
            {
                repository.Delete(system);
            }

        }
    }
}
