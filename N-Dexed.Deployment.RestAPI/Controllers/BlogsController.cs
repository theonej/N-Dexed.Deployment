using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Blog;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.Common.Domain.SystemInstructions.Handlers;
using N_Dexed.Deployment.RestAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class BlogsController : ApiController
    {
        private readonly IRepository<BlogInfo> m_BlogRepository;
        private readonly IMessageLogger m_Logger;

        public BlogsController(IRepository<BlogInfo> blogRepository, 
                               IMessageLogger logger)
        {
            Condition.Requires(blogRepository).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            m_BlogRepository = blogRepository;
            m_Logger = logger;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                BlogInfo searchCriteria = new BlogInfo();

                List<BlogInfo> blogs = m_BlogRepository.Search(searchCriteria);
                response = Request.CreateResponse(HttpStatusCode.OK, blogs);
            }
            catch (Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                BlogInfo searchCriteria = new BlogInfo();
                searchCriteria.Id = id;

                List<BlogInfo> blogs = m_BlogRepository.Search(searchCriteria);
                response = Request.CreateResponse(HttpStatusCode.OK, blogs);
            }
            catch (Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        [HttpPost]
        [ApiAuthorizationFilter(RequiredPermission = "AddBlogEntry")]
        public HttpResponseMessage Post(AddBlogEntryInstruction instruction)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                BlogInstructionHandler handler = new BlogInstructionHandler(m_BlogRepository);
                handler.Handle(instruction);

                response = Request.CreateResponse(HttpStatusCode.Created, instruction.Id);
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
