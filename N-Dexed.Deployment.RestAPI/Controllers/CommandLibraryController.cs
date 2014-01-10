using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.Storage;
using N_Dexed.Deployment.Common.Domain.SystemInstructions;
using N_Dexed.Deployment.Common.Domain.SystemInstructions.Handlers;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.RestAPI.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace N_Dexed.Deployment.RestAPI.Controllers
{
    public class CommandLibraryController : ApiController
    {
        private string TEMP_PATH = Path.GetTempPath();
        private const string CREATE_COMMAND_LIBRARY_INSTRUCTION_FORM_DATA_NAME = "createCommandLibraryInstruction";

        private readonly IStorageInterface<CommandLibraryInfo> m_StorageInterface;
        private readonly IRepository<CommandLibraryInfo> m_LibraryRepository;
        private readonly IMessageLogger m_Logger;

        public CommandLibraryController(IStorageInterface<CommandLibraryInfo> storageInterface, 
                                        IRepository<CommandLibraryInfo> libraryRepository,
                                        IMessageLogger logger)
        {
            Condition.Requires(storageInterface).IsNotNull();
            Condition.Requires(libraryRepository).IsNotNull();
            Condition.Requires(logger).IsNotNull();

            m_StorageInterface = storageInterface;
            m_LibraryRepository = libraryRepository;
            m_Logger = logger;
        }

        [HttpGet]
        [ApiAuthorizationFilter]
        public HttpResponseMessage Get(Guid customerId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                CommandLibraryInfo searchCriteria = new CommandLibraryInfo();
                searchCriteria.CustomerId = customerId;

                List<CommandLibraryInfo> commandLibraries = m_LibraryRepository.Search(searchCriteria);
                response = Request.CreateResponse(HttpStatusCode.OK, commandLibraries);
            }
            catch (Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        [HttpPost]
        [ApiAuthorizationFilter(RequiredPermission="CreateCommandLibrary")]
        public async Task<HttpResponseMessage> Post()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);

            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new DataMisalignedException(ErrorMessages.MultiPartMIMEDataExpected);
                }

                MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(TEMP_PATH);
                await Request.Content.ReadAsMultipartAsync(provider);

                //get file from post data
                FileInfo commandLibraryFile = GetCommandLibraryFile(provider);
                CreateCommandLibraryInstruction formData = GetCommandLibraryFormData(provider);

                string fileKey = string.Format("{0}/{1}/{2}", formData.CustomerId, formData.Id, formData.LibraryName);

                //save to storage context
                formData.LibraryUri = m_StorageInterface.SaveFile(fileKey, commandLibraryFile);

                //create repository record
                ISystemInstructionHandler<CreateCommandLibraryInstruction> handler = new CommandLibraryInstructionHandler(m_LibraryRepository, m_Logger);
                handler.Handle(formData);

                //clean up resources
                commandLibraryFile.Delete();

                //return repository record Id/location
                response = Request.CreateResponse(HttpStatusCode.Created, formData.LibraryUri);
            }
            catch (DataMisalignedException ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch(Exception ex)
            {
                m_Logger.WriteException(ex);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        #region Private Methods

        private FileInfo GetCommandLibraryFile(MultipartFormDataStreamProvider provider)
        {
            if (provider.FileData == null || provider.FileData.Count == 0)
            {
                throw new DataMisalignedException(ErrorMessages.NoFileDataProvided);
            }

            MultipartFileData fileData = provider.FileData[0];

            FileInfo returnValue = new FileInfo(fileData.LocalFileName);

            return returnValue;
        }

        private CreateCommandLibraryInstruction GetCommandLibraryFormData(MultipartFormDataStreamProvider provider)
        {
            if (provider.FormData == null || provider.FormData.Count == 0)
            {
                throw new DataMisalignedException(ErrorMessages.NoFileDataProvided);
            }

            string formData =  provider.FormData[CREATE_COMMAND_LIBRARY_INSTRUCTION_FORM_DATA_NAME];
            if (string.IsNullOrEmpty(formData))
            {
                throw new DataMisalignedException(ErrorMessages.NoFileDataProvided);
            }

            CreateCommandLibraryInstruction instruction = JsonConvert.DeserializeObject<CreateCommandLibraryInstruction>(formData);

            if (instruction.Id == Guid.Empty)
            {
                instruction.Id = Guid.NewGuid();
            }

            return instruction;
        }

        #endregion
    }
}
