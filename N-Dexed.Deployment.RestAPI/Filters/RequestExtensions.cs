using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Web;
using System.Web.Http.Routing;
using N_Dexed.Deployment.Common.Resources;

namespace N_Dexed.Deployment.RestAPI.Filters
{
    public static class RequestExtensions
    {
        internal const string SECURITY_TOKEN_NAME = "n-dexed.security.token";
        private const string CUSTOMER_ID_ROUTE_PARAMETER = "customerId";

        public static string GetAuthenticationToken(this HttpRequestMessage request)
        {
            string returnValue = null;

            IEnumerable<string> values = new List<string>();
            request.Headers.TryGetValues(SECURITY_TOKEN_NAME, out values);

            if(values != null)
            {
                returnValue = values.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(returnValue))
            {
                throw new MissingFieldException(ErrorMessages.SecurityTokenNotFound);
            }

            return returnValue;
        }

        public static Guid? GetCustomerId(this HttpRequestMessage request)
        {
            Guid? customerId = null;

            IHttpRouteData routeData =  request.GetRouteData();

            if (routeData.Values.ContainsKey(CUSTOMER_ID_ROUTE_PARAMETER) && routeData.Values[CUSTOMER_ID_ROUTE_PARAMETER] != null)
            {
                customerId = Guid.Parse(routeData.Values[CUSTOMER_ID_ROUTE_PARAMETER].ToString());
            }

            return customerId;
        }
    }
}