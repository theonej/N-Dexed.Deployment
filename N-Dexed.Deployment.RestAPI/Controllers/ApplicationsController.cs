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
    public class ApplicationsController : ApiController
    {
        private readonly IApplicationInterface m_ApplicationInterface;
        private readonly IMessageLogger m_Logger;

        public ApplicationsController(IApplicationInterface applicationInterface,
                                      IMessageLogger messageLogger)
        {
            Condition.Requires(applicationInterface).IsNotNull();
            Condition.Requires(messageLogger).IsNotNull();

            m_ApplicationInterface = applicationInterface;
            m_Logger = messageLogger; 
        }

        [HttpGet]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Get (Guid customerId, Guid systemId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                List<ApplicationInfo> applications = m_ApplicationInterface.GetApplications(systemId);

                response = Request.CreateResponse(HttpStatusCode.OK, applications);
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
