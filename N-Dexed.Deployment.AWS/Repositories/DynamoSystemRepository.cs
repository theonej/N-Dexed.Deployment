using System;
using System.Collections.Generic;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Newtonsoft.Json;

using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Security;

namespace N_Dexed.Deployment.AWS.Repositories
{
    public class DynamoSystemRepository : IRepository<SystemInfo>
    {
        private const string SYSTEMS_TABLE_NAME = "n-dexed.deployment.systems";
        private const string SYSTEM_ID_COLUMN = "SystemId";
        private const string CUSTOMER_ID_COLUMN = "CustomerId";
        
        private readonly IMessageLogger m_Logger;
        private readonly IEncryptor m_Encryptor;

        public DynamoSystemRepository(IMessageLogger logger, IEncryptor encryptor)
        {
            CuttingEdge.Conditions.Condition.Requires<IMessageLogger>(logger);
            CuttingEdge.Conditions.Condition.Requires<IEncryptor>(encryptor);

            m_Logger = logger;
            m_Encryptor = encryptor;
        }

        public SystemInfo Get(SystemInfo item)
        {
            AmazonDynamoDBClient client =  DynamoUtilities.InitializeClient();
            using (client)
            {
                GetItemRequest request = CreateGetItemRequest(item);

                GetItemResponse response = client.GetItem(request);

                if (response.Item == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Get System");
                    throw new MissingFieldException(errorMessage);
                }

                SystemInfo returnValue = DynamoUtilities.GetItemFromAttributeStore<SystemInfo>(response.Item);
                returnValue.Credentials.AccessKey = m_Encryptor.DecryptValue(returnValue.Credentials.AccessKey);
                returnValue.Credentials.SecretKey = m_Encryptor.DecryptValue(returnValue.Credentials.SecretKey);

                return returnValue;
            }
        }

        public Guid Save(SystemInfo item)
        {
            if (item.Id == Guid.Empty)
            {
                item.Id = Guid.NewGuid();
            }

            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                PutItemRequest request = CreatePutItemRequest(item);

                try
                {
                    client.PutItem(request);

                    return item.Id;
                }
                catch (AmazonDynamoDBException ex)
                {
                    m_Logger.WriteException(ex);

                    throw new MissingFieldException(ex.Message);
                }
            }
        }

        public List<SystemInfo> Search(SystemInfo searchCriteria)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                ScanRequest request = CreateScanRequest(searchCriteria);

                ScanResponse response = client.Scan(request);

                if (response.Items == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "System");
                    throw new MissingFieldException(errorMessage);
                }

                List<SystemInfo> returnValue = new List<SystemInfo>();
                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    SystemInfo system = DynamoUtilities.GetItemFromAttributeStore<SystemInfo>(item);
                    system.Credentials.AccessKey = m_Encryptor.DecryptValue(system.Credentials.AccessKey);
                    system.Credentials.SecretKey = m_Encryptor.DecryptValue(system.Credentials.SecretKey);
                    returnValue.Add(system);
                }

                return returnValue;
            }
        }

        public void Delete(SystemInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                DeleteItemRequest request = CreateDeleteItemRequest(item);

                client.DeleteItem(request);
            }
        }

        #region Private Methods

        private static GetItemRequest CreateGetItemRequest(SystemInfo item)
        {
            GetItemRequest request = new GetItemRequest();

            request.TableName = SYSTEMS_TABLE_NAME;
            request.Key = new Dictionary<string, AttributeValue>
            {
                {SYSTEM_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)},
                {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId)}
            };

            return request;
        }

        private PutItemRequest CreatePutItemRequest(SystemInfo item)
        {
            PutItemRequest request = new PutItemRequest();

            request.TableName = SYSTEMS_TABLE_NAME;

            request.Item = new Dictionary<string, AttributeValue>();

            item.Credentials.AccessKey = m_Encryptor.EncryptValue(item.Credentials.AccessKey);
            item.Credentials.SecretKey = m_Encryptor.EncryptValue(item.Credentials.SecretKey);

            request.Item.Add(SYSTEM_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id));
            request.Item.Add(CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId));
            request.Item.Add(DynamoUtilities.SERIALIZED_DATA_COLUMN, DynamoUtilities.GetItemAttributeSerializedValue(item));

            return request;
        }

        private static DeleteItemRequest CreateDeleteItemRequest(SystemInfo item)
        {
            DeleteItemRequest request = new DeleteItemRequest();

            request.TableName = SYSTEMS_TABLE_NAME;
            request.Key = new Dictionary<string, AttributeValue>()
            { 
              {SYSTEM_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)},
              {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId)}
            };

            return request;
        }

        private ScanRequest CreateScanRequest(SystemInfo searchCriteria)
        {
            ScanRequest request = new ScanRequest();

            request.TableName = SYSTEMS_TABLE_NAME;
            request.ScanFilter = new Dictionary<string, Condition>();
            if(searchCriteria.CustomerId != Guid.Empty)
            {
                Condition customerIdCondition = new Condition() 
                        {
                            ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR, 
                            AttributeValueList = new List<AttributeValue>()
                            {
                                DynamoUtilities.GetItemAttributeStringValue(searchCriteria.CustomerId)
                            }
                        };

                request.ScanFilter.Add(CUSTOMER_ID_COLUMN, customerIdCondition);
            }

            if (searchCriteria.Id != Guid.Empty)
            {
                Condition systemIdCondition = new Condition()
                {
                    ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR,
                    AttributeValueList = new List<AttributeValue>()
                            {
                                DynamoUtilities.GetItemAttributeStringValue(searchCriteria.Id)
                            }
                };

                request.ScanFilter.Add(SYSTEM_ID_COLUMN, systemIdCondition);
            }

            return request;
        }

        #endregion
    }
}
