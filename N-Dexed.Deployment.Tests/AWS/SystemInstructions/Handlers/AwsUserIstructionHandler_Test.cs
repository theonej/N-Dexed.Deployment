using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.AWS.SystemInstructions.Handlers;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.SystemInstructions;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Security.Hashers;

namespace N_Dexed.Deployment.Tests.AWS.SystemInstructions.Handlers
{
    [TestClass]
    public class AwsUserIstructionHandler_Test
    {
        [TestMethod]
        public void CreateNewUserAndNewCustomerThenDelete()
        {
            CreateUserInstruction instruction = new CreateUserInstruction();

            instruction.CustomerName = "Test Customer";
            instruction.EmailAddress = "test.user@example.com";
            instruction.Password = "test.password";
            instruction.UserName = "test.user";

            IRepository<CustomerInfo> customerRepository = new DynamoCustomerRepository(new DynamoMessageLogger());
            IRepository<UserInfo> userRepository = new DynamoUserRepository(new DynamoMessageLogger());
            IHashProvider hashProvider = new  PublicPrivateKeyHasher();
            AwsUserIstructionHandler handler = new AwsUserIstructionHandler(customerRepository, userRepository, hashProvider);

            try
            {
                handler.Handle(instruction);
            }
            finally
            {
                UserInfo searchCriteria = new UserInfo();
                searchCriteria.EmailAddress = instruction.EmailAddress;

                List<UserInfo> users = userRepository.Search(searchCriteria);
                if (users.Count > 0)
                {
                    UserInfo userInfo = users[0];
                    userRepository.Delete(userInfo);

                    CustomerInfo customerSearchCriteria = new CustomerInfo();
                    customerSearchCriteria.CustomerName = instruction.CustomerName;

                    List<CustomerInfo> customers = customerRepository.Search(customerSearchCriteria);
                    Assert.AreNotEqual(0, customers.Count);
                    CustomerInfo savedCustomer = customers[0];
                    customerRepository.Delete(savedCustomer);
                }

            }
        }

        [TestMethod]
        [ExpectedException (typeof(DataMisalignedException))]
        public void CreateNewUserAndNewCustomerThenTryToCreateDuplicateDelete()
        {
            CreateUserInstruction instruction = new CreateUserInstruction();

            instruction.CustomerName = "Test Customer";
            instruction.EmailAddress = "test.user@example.com";
            instruction.Password = "test.password";
            instruction.UserName = "test.user";

            IRepository<CustomerInfo> customerRepository = new DynamoCustomerRepository(new DynamoMessageLogger());
            IRepository<UserInfo> userRepository = new DynamoUserRepository(new DynamoMessageLogger());
            IHashProvider hashProvider = new PublicPrivateKeyHasher();
            AwsUserIstructionHandler handler = new AwsUserIstructionHandler(customerRepository, userRepository, hashProvider);

            try
            {
                handler.Handle(instruction);

                //even changing the case of the custoemr name should cause a failure
                instruction.CustomerName = instruction.CustomerName.ToUpper();

                //the next call should fail
                handler.Handle(instruction);
            }
            finally
            {
                UserInfo searchCriteria = new UserInfo();
                searchCriteria.EmailAddress = instruction.EmailAddress;

                List<UserInfo> users = userRepository.Search(searchCriteria);
                if (users.Count > 0)
                {
                    UserInfo userInfo = users[0];
                    userRepository.Delete(userInfo);

                    CustomerInfo customerSearchCriteria = new CustomerInfo();
                    customerSearchCriteria.CustomerName = instruction.CustomerName;

                    List<CustomerInfo> customers = customerRepository.Search(customerSearchCriteria);
                    Assert.AreNotEqual(0, customers.Count);
                    CustomerInfo savedCustomer = customers[0];
                    customerRepository.Delete(savedCustomer);
                }
            }
        }
    }
}
