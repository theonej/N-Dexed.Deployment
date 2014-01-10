using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Web.Http;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Security;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class AuthorizationController : ApiController
    {
        const string BASIC_AUTH_SCHEME = "basic";

        private readonly IAuthorizationTokenProvider m_TokenProvider;
        private readonly IRepository<UserInfo> m_UserRepository;
        private readonly IHashProvider m_HashProvider;
        private readonly IMessageLogger m_MessageLogger;

        public AuthorizationController(IAuthorizationTokenProvider tokenProvider, 
                                       IRepository<UserInfo> userRepository,
                                       IHashProvider hashProvider,
                                       IMessageLogger messageLogger)
        {
            Condition.Requires(tokenProvider).IsNotNull();
            Condition.Requires(userRepository).IsNotNull();
            Condition.Requires(hashProvider).IsNotNull();
            Condition.Requires(messageLogger).IsNotNull();

            m_TokenProvider = tokenProvider;
            m_UserRepository = userRepository;
            m_HashProvider = hashProvider;
            m_MessageLogger = messageLogger;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                //get the basic auth credentials
                string basicAuthHeader = GetBasicAuthValue(Request.Headers.Authorization);

                //find a user that matches
                UserInfo authenticatedUser = GetUser(basicAuthHeader);

                //create security token
                string securityToken = m_TokenProvider.GenerateAuthorizationToken(authenticatedUser.Id);

                response = Request.CreateResponse(HttpStatusCode.OK, securityToken);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            }
            catch (Exception ex)
            {
                m_MessageLogger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        #region Private Methods

        private string GetBasicAuthValue(AuthenticationHeaderValue value)
        {
            string providedScheme = value.Scheme.ToLower();
            if (providedScheme != BASIC_AUTH_SCHEME)
            {
                string errorMessage = string.Format(ErrorMessages.AuthenticationSchemeNotSupported, providedScheme);
                throw new NotSupportedException(errorMessage);
            }

           byte[] valueBytes = Convert.FromBase64String(value.Parameter);

           string returnValue = Encoding.UTF8.GetString(valueBytes);

           return returnValue;
        }

        private UserInfo GetUser(string basicAuthHeader)
        {
            string[] authParts = basicAuthHeader.Split(':');
            if (authParts.Length != 2)
            {
                throw new FormatException(ErrorMessages.CredentialFormatError);
            }

            UserInfo searchCriteria = new UserInfo();
            searchCriteria.EmailAddress = authParts[0];

            UserInfo userData = m_UserRepository.Search(searchCriteria).FirstOrDefault();
            if (userData == null)
            {
                throw new AuthenticationException(ErrorMessages.UserNotFound);
            }

            string passwordHash = GetUserPasswordHash(authParts[1], userData.CreatedDateTime);
            if (userData.PasswordHash != passwordHash)
            {
                throw new AuthenticationException(ErrorMessages.UserNotFound);
            }

            return userData;
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
