using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.RestAPI.Filters;
using N_Dexed.Deployment.Security;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IRepository<UserInfo> m_UserRepository;
        private readonly IMessageLogger m_MessageLogger;
        private readonly IAuthorizationTokenProvider m_TokenProvider;

        public UsersController(IRepository<UserInfo> userRepository,
                               IMessageLogger messageLogger,
                               IAuthorizationTokenProvider tokenProvider)
        {
            Condition.Requires(userRepository).IsNotNull();
            Condition.Requires(messageLogger).IsNotNull();
            Condition.Requires(tokenProvider).IsNotNull();

            m_UserRepository = userRepository;
            m_MessageLogger = messageLogger;
            m_TokenProvider = tokenProvider;
        }

        [HttpGet]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                UserInfo searchCriteria = new UserInfo();
                searchCriteria.EmailAddress = id;

                UserInfo userData = m_UserRepository.Search(searchCriteria).FirstOrDefault();
                if (userData == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                response = Request.CreateResponse(HttpStatusCode.OK, userData);
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

        [HttpGet]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                string securityToken = Request.GetAuthenticationToken();
                Guid userId = m_TokenProvider.ValidateAuthorizationToken(securityToken);
                UserInfo searchCriteria = new UserInfo();
                searchCriteria.Id = userId;

                UserInfo userData = m_UserRepository.Search(searchCriteria).FirstOrDefault();
                if (userData == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                response = Request.CreateResponse(HttpStatusCode.OK, userData);
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
