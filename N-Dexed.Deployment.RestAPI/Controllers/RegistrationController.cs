using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Web.Http;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Common.SystemInstructions;
using N_Dexed.Deployment.Security;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class RegistrationController : ApiController
    {
        private readonly ISystemInstructionHandler<CreateUserInstruction> m_InstructionHandler;
        private readonly IRepository<UserInfo> m_UserRepository;
        private readonly IMessageLogger m_Logger;
        private readonly IAuthorizationTokenProvider m_TokenProvider;

        public RegistrationController(ISystemInstructionHandler<CreateUserInstruction> instructionHandler, 
                                      IRepository<UserInfo> userRepository,
                                      IMessageLogger logger,
                                      IAuthorizationTokenProvider tokenProvider)
        {
            Condition.Requires(instructionHandler).IsNotNull();
            Condition.Requires(userRepository).IsNotNull();
            Condition.Requires(logger).IsNotNull();
            Condition.Requires(tokenProvider).IsNotNull();

            m_InstructionHandler = instructionHandler;
            m_UserRepository = userRepository;
            m_Logger = logger;
            m_TokenProvider = tokenProvider;
        }

        /// <summary>
        /// Can be used to check quickly for the existence of a user with the same email address.
        /// an error will be thrown if the user exists (419 - Conflict)
        /// </summary>
        /// <param name="userEmailAddress"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);
   
            try
            {
                UserInfo searchCriteria = new UserInfo();
                searchCriteria.EmailAddress = id;

                List<UserInfo> users = m_UserRepository.Search(searchCriteria);
                if (users != null && users.Any())
                {
                    UserInfo existingUser = users[0];

                    response = Request.CreateResponse(HttpStatusCode.Conflict, ErrorMessages.UserExists);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Post(CreateUserInstruction instruction)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                m_InstructionHandler.Handle(instruction);

                UserInfo searchCriteria = new UserInfo();
                searchCriteria.EmailAddress = instruction.EmailAddress;

                UserInfo userData = m_UserRepository.Search(searchCriteria).FirstOrDefault();
                if (userData == null)
                {
                    throw new AuthenticationException(ErrorMessages.UserNotFound);
                }

                string authToken = m_TokenProvider.GenerateAuthorizationToken(userData.Id);
                response = Request.CreateResponse(HttpStatusCode.Created, authToken);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            }
            catch (DataMisalignedException ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (MissingFieldException)//this should already be being logged by the implementation
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ErrorMessages.MissingUserData);
            }
            catch (Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }
    }
}
