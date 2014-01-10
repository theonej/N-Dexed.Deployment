using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Management;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.RestAPI.Filters;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class EnvironmentsController : ApiController
    {
        private readonly IApplicationInterface m_ApplicationInterface;
        private readonly IMessageLogger m_Logger;

        public EnvironmentsController(IApplicationInterface applicationInterface,
                                      IMessageLogger logger)
        {
            Condition.Requires(applicationInterface).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            m_ApplicationInterface = applicationInterface;
            m_Logger = logger;
        }

        [HttpGet]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Get(Guid customerId, Guid systemId, string applicationName)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                List<EnvironmentInfo> environments = m_ApplicationInterface.GetEnvironments(systemId, applicationName);

                response = Request.CreateResponse(HttpStatusCode.OK, environments);
            }
            catch (Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }
    }
}
