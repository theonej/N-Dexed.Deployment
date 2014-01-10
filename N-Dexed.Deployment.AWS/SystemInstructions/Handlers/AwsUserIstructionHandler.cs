using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Common.SystemInstructions;
using N_Dexed.Deployment.Security;

namespace N_Dexed.Deployment.AWS.SystemInstructions.Handlers
{
    public class AwsUserIstructionHandler : ISystemInstructionHandler<CreateUserInstruction>
    {
        private readonly IRepository<CustomerInfo> m_CustomerRepository;
        private readonly IRepository<UserInfo> m_UserRepository;
        private readonly IHashProvider m_HashProvider;

        public AwsUserIstructionHandler(IRepository<CustomerInfo> customerRepository,
                                              IRepository<UserInfo> userRepository,
                                              IHashProvider hashProvider)
        {
            Condition.Requires(customerRepository).IsNotNull();
            Condition.Requires(userRepository).IsNotNull();
            Condition.Requires(hashProvider).IsNotNull();

            m_CustomerRepository = customerRepository;
            m_UserRepository = userRepository;
            m_HashProvider = hashProvider;
        }

        /// <summary>
        /// Workflow:
        ///     1. Create Customer
        ///     2. Hash Password
        ///     3. Create User
        /// </summary>
        /// <param name="instruction"></param>
        public void Handle(CreateUserInstruction instruction)
        {
            //create the customer.  If the customer exists, throw an exception
            Guid customerId = CreateCustomer(instruction.CustomerName, instruction.EmailAddress);
            
            //persist the user.  If a user with the same email address exists, throw an exception
            CreateUserFromInstruction(instruction, customerId);
        }

        #region Private Methods

        private Guid CreateCustomer(string customerName, string userEmailAddress)
        {
            CustomerInfo searchCriteria = new CustomerInfo();
            searchCriteria.CustomerName = customerName;

            List<CustomerInfo> customers = m_CustomerRepository.Search(searchCriteria);
            if(customers != null && customers.Any())
            {
                string errorMessage = string.Format(ErrorMessages.CustomerNameInUse, customerName);
                throw new DataMisalignedException(errorMessage);
            }

            CustomerInfo newCustomer = new CustomerInfo();
            newCustomer.CustomerName = customerName;
            newCustomer.AdminEmailAddress = userEmailAddress;
            newCustomer.Id = m_CustomerRepository.Save(newCustomer);

            return newCustomer.Id;
        }

        private void CreateUserFromInstruction(CreateUserInstruction instruction, Guid customerId)
        {
            UserInfo newUser = new UserInfo();
            newUser.EmailAddress = instruction.EmailAddress;

            List<UserInfo> users = m_UserRepository.Search(newUser);
            if (users != null && users.Any())
            {
                string errorMessage = string.Format(ErrorMessages.UserEmailInUse, newUser.EmailAddress);
                throw new DataMisalignedException(errorMessage);
            }

            newUser.CreatedDateTime = DateTime.Now;
            newUser.CustomerId = customerId;
            newUser.PasswordHash = GetUserPasswordHash(instruction.Password, newUser.CreatedDateTime);
            newUser.UserName = instruction.UserName;

            m_UserRepository.Save(newUser);
        }

        private string GetUserPasswordHash(string password, DateTime userCreatedDateTime)
        {
            string publicKey = ConfigurationManager.AppSettings["PublicKey"];

            string passwordHash = m_HashProvider.GenerateHash(password, userCreatedDateTime.ToString(), publicKey);

            return passwordHash;
        }

        #endregion
    }
}
