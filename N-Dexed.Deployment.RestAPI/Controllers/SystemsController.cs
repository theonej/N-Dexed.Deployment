using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CuttingEdge.Conditions;

using N_Dexed.Deployment.RestAPI.Filters;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Security;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class SystemsController : ApiController
    {
        private readonly IRepository<SystemInfo> m_SystemRepository;
        private readonly IRepository<UserInfo> m_UserRepository;
        private readonly IMessageLogger m_MessageLogger;
        private readonly ISystemInstructionHandler<CreateSystemInstruction> m_InstructionHandler;

        public SystemsController(IRepository<SystemInfo> systemRepository,
                                 IRepository<UserInfo> userRepository,
                                 IMessageLogger messageLogger,
                                 ISystemInstructionHandler<CreateSystemInstruction> instructionHandler)
        {
            Condition.Requires(systemRepository).IsNotNull();
            Condition.Requires(userRepository).IsNotNull();
            Condition.Requires(messageLogger).IsNotNull();
            Condition.Requires(instructionHandler).IsNotNull();

            m_UserRepository = userRepository;
            m_MessageLogger = messageLogger;
            m_SystemRepository = systemRepository;
            m_InstructionHandler = instructionHandler;
        }

        [HttpGet]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Get(Guid customerId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                SystemInfo searchCriteria = new SystemInfo();
                searchCriteria.CustomerId = customerId;

                List<SystemInfo> systems = m_SystemRepository.Search(searchCriteria);

                foreach (SystemInfo system in systems)
                {
                    //this information is not for public consumption
                    system.Credentials = new AccessCredentials();
                }

                response = Request.CreateResponse(HttpStatusCode.OK, systems);
            }
            catch (KeyNotFoundException ex)
            {
                m_MessageLogger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                m_MessageLogger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }


        [HttpPost]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Post(CreateSystemInstruction instruction)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                m_InstructionHandler.Handle(instruction);
                response = Request.CreateResponse(HttpStatusCode.Accepted, instruction.Id);
            }
            catch (KeyNotFoundException ex)
            {
                m_MessageLogger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                m_MessageLogger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }
    }
}
