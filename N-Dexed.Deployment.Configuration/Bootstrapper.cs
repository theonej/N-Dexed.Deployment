using System;
using System.Collections.Generic;

using N_Dexed.Deployment.Common.Domain.Commands;
using N_Dexed.Deployment.AWS.Commands;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain;
using N_Dexed.Deployment.AWS.Repositories;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Security.Hashers;
using N_Dexed.Deployment.Security.Encryptors;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.AWS.SystemInstructions.Handlers;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.Common.SystemInstructions;
using N_Dexed.Deployment.Security.Providers;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Common.Domain.Management;
using N_Dexed.Deployment.AWS.Management;
using N_Dexed.Deployment.Common.Domain.Storage;
using N_Dexed.Deployment.AWS.Storage;
using N_Dexed.Deployment.Common.Domain.Blog;

namespace N_Dexed.Deployment.Configuration
{
    public static class Bootstrapper
    {
        public static DependencyController CreateDependencyController()
        {
            DependencyController controller = new DependencyController();

            controller.RegisterTypes();

            return controller;
        }

        /// <summary>
        /// Register your concrete implementations here
        /// </summary>
        /// <param name="controller"></param>
        private static void RegisterTypes(this DependencyController controller)
        {
            //command processors
            controller.Register<ICommandProcessor, AwsCommandProcessor>();

            //repositories
            controller.Register<IRepository<CommandLibraryInfo>, DynamoCommandLibraryRepository>();
            controller.Register<IRepository<CustomerInfo>, DynamoCustomerRepository>();
            controller.Register<IRepository<UserInfo>, DynamoUserRepository>();
            controller.Register<IRepository<SystemInfo>, DynamoSystemRepository>();
            controller.Register<IRepository<BlogInfo>, DynamoBlogRepository>();

            //storage interfaces
            controller.Register<IStorageInterface<CommandLibraryInfo>, S3CommandLibraryStorageInterface>();

            //security
            controller.Register<IHashProvider, PublicPrivateKeyHasher>();
            controller.Register<IEncryptor, RijndaelManagedEncryptor>();
            controller.Register<IAuthorizationTokenProvider, HashAuthorizationTokenProvider>();

            //instruction handlers
            controller.Register<ISystemInstructionHandler<CreateUserInstruction>, AwsUserIstructionHandler>();
            controller.Register<ISystemInstructionHandler<CreateSystemInstruction>, AwsSystemsInstructionHandler>();

            //interfaces
            controller.Register<IApplicationInterface, ElasticBeanstalkApplicationInterface>();

            //logging
            controller.Register<IMessageLogger, DynamoMessageLogger>();
        }
    }
}
