using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Repositories;

namespace N_Dexed.Deployment.Tests.AWS
{
    [TestClass]
    public class DynamoCustomerRepository_Test
    {
        [TestMethod]
        public void CreateNewCustomerThenDelete()
        {
            IRepository<CustomerInfo> customerRepository = new DynamoCustomerRepository(new DynamoMessageLogger());

            CustomerInfo customer = new CustomerInfo();
            customer.Id = Guid.NewGuid();
            customer.CustomerName = "Test Customer 1";
            customer.Credentials = new AccessCredentials();
            customer.Credentials.AccessKey = "No Access Key";
            customer.Credentials.SecretKey = "No Secret Key";
            
            try
            {
                customerRepository.Save(customer);
            }
            finally
            {
                customerRepository.Delete(customer);
            }
        }

        [TestMethod]
        public void CreateNewCustomerThenGetThenDelete()
        {
            IRepository<CustomerInfo> customerRepository = new DynamoCustomerRepository(new DynamoMessageLogger());

            CustomerInfo customer = new CustomerInfo();
            customer.Id = Guid.NewGuid();
            customer.CustomerName = "Test Customer 1";
            customer.Credentials = new AccessCredentials();
            customer.Credentials.AccessKey = "No Access Key";
            customer.Credentials.SecretKey = "No Secret Key";

            try
            {
                customerRepository.Save(customer);

                CustomerInfo savedCustomer = customerRepository.Get(customer);
                Assert.IsNotNull(savedCustomer);
                Assert.AreEqual(savedCustomer.CustomerName, customer.CustomerName);
                Assert.AreEqual(savedCustomer.Id, customer.Id);
            }
            finally
            {
                customerRepository.Delete(customer);
            }
        }

        [TestMethod]
        public void CreateNewCustomerThenSearchByCustomerNameThenDelete()
        {
            IRepository<CustomerInfo> customerRepository = new DynamoCustomerRepository(new DynamoMessageLogger());

            CustomerInfo customer = new CustomerInfo();
            customer.Id = Guid.NewGuid();
            customer.CustomerName = "Test Customer 1";
            customer.Credentials = new AccessCredentials();
            customer.Credentials.AccessKey = "No Access Key";
            customer.Credentials.SecretKey = "No Secret Key";

            try
            {
                customerRepository.Save(customer);

                CustomerInfo searchCriteria = new CustomerInfo();
                searchCriteria.CustomerName = customer.CustomerName;

                List<CustomerInfo> customers = customerRepository.Search(searchCriteria);
                Assert.AreNotEqual(0, customers.Count);
                CustomerInfo savedCustomer = customers[0];

                Assert.IsNotNull(savedCustomer);
                Assert.AreEqual(savedCustomer.CustomerName, customer.CustomerName);
                Assert.AreEqual(savedCustomer.Id, customer.Id);
            }
            finally
            {
                customerRepository.Delete(customer);
            }
        }
    }
}
