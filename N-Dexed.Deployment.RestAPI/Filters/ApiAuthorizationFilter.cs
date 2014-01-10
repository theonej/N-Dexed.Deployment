using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Security;

namespace N_Dexed.Deployment.RestAPI.Filters
{
    public class ApiAuthorizationFilter : AuthorizationFilterAttribute
    {
        public string RequiredPermission { get; set; }

        private readonly IAuthorizationTokenProvider m_TokenProvider;
        private readonly IRepository<UserInfo> m_UserRepository;

        public ApiAuthorizationFilter()
        {
            m_TokenProvider = (IAuthorizationTokenProvider)WebApiConfig.GetDependencyContainer().GetService(typeof(IAuthorizationTokenProvider));
            m_UserRepository = (IRepository<UserInfo>)WebApiConfig.GetDependencyContainer().GetService(typeof(IRepository<UserInfo>));
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            try
            {
                string authToken = actionContext.Request.GetAuthenticationToken();

                Guid userId = m_TokenProvider.ValidateAuthorizationToken(authToken);
                UserInfo userData = GetRequestUser(userId);

                ValidateCustomerId(userData, actionContext);
                ValidateRequiredPermission(userData);
            }
            catch (MissingFieldException ex)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
                actionContext.Response.ReasonPhrase = ex.Message;
            }
            catch (FormatException ex)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
                actionContext.Response.ReasonPhrase = ex.Message;
            }
            catch (AuthenticationException ex)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
                actionContext.Response.ReasonPhrase = ex.Message;
            }

        }

        private UserInfo GetRequestUser(Guid userId)
        {
            UserInfo searchCriteria = new UserInfo();
            searchCriteria.Id = userId;

            UserInfo userData = m_UserRepository.Search(searchCriteria).FirstOrDefault();
            if (userData == null)
            {
                throw new KeyNotFoundException(ErrorMessages.UserNotFound);
            }

            return userData;
        }

        private void ValidateCustomerId(UserInfo userData, HttpActionContext actionContext)
        {
            Guid? customerId = actionContext.Request.GetCustomerId();
            if (customerId != null)
            {
                if (userData.CustomerId != customerId)
                {
                    throw new AuthenticationException(ErrorMessages.UserNotAuthorizedForCustomer);
                }
            }
        }

        private void ValidateRequiredPermission(UserInfo userData)
        {
            if (!string.IsNullOrEmpty(RequiredPermission))
            {
                string userPermission = (
                                            from
                                                permissions
                                            in
                                                userData.UserPermissions
                                            where
                                                permissions == RequiredPermission
                                            select
                                                permissions
                                        ).FirstOrDefault();

                if (string.IsNullOrEmpty(userPermission))
                {
                    throw new AuthenticationException(ErrorMessages.UserNotAuthorizedForCustomer);
                }
            }
        }
    }
}