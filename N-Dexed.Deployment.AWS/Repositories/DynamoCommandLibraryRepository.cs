using System;
using System.Collections.Generic;
using System.Configuration;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Newtonsoft.Json;

using N_Dexed.Deployment.Common.Domain;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;

namespace N_Dexed.Deployment.AWS.Repositories
{
    public class DynamoCommandLibraryRepository : IRepository<CommandLibraryInfo>
    {
        private const string COMMAND_LIBRARY_TABLE_NAME = "n-dexed.deployment.commandLibraries";
        private const string COMMAND_LIBRARY_ID_COLUMN = "CommandLibraryId";
        private const string CUSTOMER_ID_COLUMN = "CustomerId";

        public CommandLibraryInfo Get(CommandLibraryInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                GetItemRequest request = CreateGetItemRequest(item);

                GetItemResponse response = client.GetItem(request);

                if (response.Item == null )
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Get Command Library");
                    throw new MissingFieldException(errorMessage);
                }

                CommandLibraryInfo returnValue = DynamoUtilities.GetItemFromAttributeStore<CommandLibraryInfo>(response.Item);

                return returnValue;
            }
        }

        public Guid Save(CommandLibraryInfo item)
        {
            if (item.Id == Guid.Empty)
            {
                item.Id = Guid.NewGuid();
            }
         
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                PutItemRequest request = CreatePutItemRequest(item);

                client.PutItem(request);

                return item.Id;
            }
        }

        public List<CommandLibraryInfo> Search(CommandLibraryInfo searchCriteria)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                ScanRequest request = CreateScanRequest(searchCriteria);

                ScanResponse response = client.Scan(request);

                if (response.Items == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Search Command Library");
                    throw new MissingFieldException(errorMessage);
                }

                List<CommandLibraryInfo> returnValue = new List<CommandLibraryInfo>();
                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    CommandLibraryInfo commandLibrary = DynamoUtilities.GetItemFromAttributeStore<CommandLibraryInfo>(item);
                    returnValue.Add(commandLibrary);
                }

                return returnValue;
            }
        }

        public void Delete(CommandLibraryInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                DeleteItemRequest request = CreateDeleteItemRequest(item);

                client.DeleteItem(request);
            }
        }

        #region Private Methods

        private PutItemRequest CreatePutItemRequest(CommandLibraryInfo commandLibrary)
        {
            PutItemRequest request = new PutItemRequest();

            request.TableName = COMMAND_LIBRARY_TABLE_NAME;

            request.Item = new Dictionary<string, AttributeValue>();

            request.Item.Add(COMMAND_LIBRARY_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(commandLibrary.Id));
            request.Item.Add(CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(commandLibrary.CustomerId));
            request.Item.Add(DynamoUtilities.SERIALIZED_DATA_COLUMN, DynamoUtilities.GetItemAttributeSerializedValue(commandLibrary));

            return request;
        }

        private ScanRequest CreateScanRequest(CommandLibraryInfo searchCriteria)
        {
            ScanRequest request = new ScanRequest();

            request.TableName = COMMAND_LIBRARY_TABLE_NAME;
            request.ScanFilter = new Dictionary<string, Condition>
            {
                {
                    CUSTOMER_ID_COLUMN,
                    new Condition() 
                    {
                        ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR, 
                        AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.CustomerId)
                        }
                    }
               }
            };

            return request;
        }

        private DeleteItemRequest CreateDeleteItemRequest(CommandLibraryInfo item)
        {
            DeleteItemRequest request = new DeleteItemRequest();

            request.TableName = COMMAND_LIBRARY_TABLE_NAME;
            request.Key = new Dictionary<string, AttributeValue>()
            { 
              {COMMAND_LIBRARY_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id) },
              {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId)}
            };

            return request;
        }

        private GetItemRequest CreateGetItemRequest(CommandLibraryInfo item)
        {
            GetItemRequest request = new GetItemRequest();

            request.TableName = COMMAND_LIBRARY_TABLE_NAME;

            request.Key = new Dictionary<string, AttributeValue>()
            { 
              {COMMAND_LIBRARY_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id) },
              {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId)}
            };

            return request;
        }

        #endregion
    }
}
