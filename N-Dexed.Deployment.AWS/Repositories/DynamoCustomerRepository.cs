using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Newtonsoft.Json;

using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;
using N_Dexed.Deployment.Common.Domain.Messaging;

namespace N_Dexed.Deployment.AWS.Repositories
{
    public class DynamoCustomerRepository : IRepository<CustomerInfo>
    {
        private readonly IMessageLogger m_Logger;

        public DynamoCustomerRepository(IMessageLogger logger)
        {
            m_Logger = logger;
        }

        private const string CUSTOMER_TABLE_NAME = "n-dexed.deployment.customers";
        private const string CUSTOMER_ID_COLUMN = "CustomerId";
        private const string CUSTOMER_NAME_COLUMN = "CustomerName";

        public CustomerInfo Get(CustomerInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                GetItemRequest request = CreateGetItemRequest(item);

                GetItemResponse response = client.GetItem(request);

                if (response.Item == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Get Customer");
                    throw new MissingFieldException(errorMessage);
                }

                CustomerInfo returnValue = DynamoUtilities.GetItemFromAttributeStore<CustomerInfo>(response.Item);

                return returnValue;
            }
        }

        public Guid Save(CustomerInfo item)
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

        public List<CustomerInfo> Search(CustomerInfo searchCriteria)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                ScanRequest request = CreateScanRequest(searchCriteria);

                try
                {
                    ScanResponse response = client.Scan(request);

                    if (response.Items == null)
                    {
                        string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Search Command Library");
                        throw new MissingFieldException(errorMessage);
                    }

                    List<CustomerInfo> returnValue = new List<CustomerInfo>();
                    foreach (Dictionary<string, AttributeValue> item in response.Items)
                    {
                        CustomerInfo customer = DynamoUtilities.GetItemFromAttributeStore<CustomerInfo>(item);
                        returnValue.Add(customer);
                    }

                    return returnValue;
                }
                catch (AmazonDynamoDBException ex)
                {
                    m_Logger.WriteException(ex);

                    throw new MissingFieldException(ex.Message);
                }
            }
        }

        public void Delete(CustomerInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                DeleteItemRequest request = CreateDeleteItemRequest(item);

                client.DeleteItem(request);
            }
        }

        #region Private Methods

        private static GetItemRequest CreateGetItemRequest(CustomerInfo item)
        {
            GetItemRequest request = new GetItemRequest();

            request.TableName = CUSTOMER_TABLE_NAME;
            request.Key = new Dictionary<string, AttributeValue>
            {
                {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)}
            };

            return request;
        }

        private static PutItemRequest CreatePutItemRequest(CustomerInfo item)
        {
            if (string.IsNullOrEmpty(item.CustomerName))
            {
                string errorMessage = string.Format(ErrorMessages.MissingRequiredAttribute, "Customer Name");
                throw new MissingFieldException(errorMessage);
            }
            PutItemRequest request = new PutItemRequest();

            request.TableName = CUSTOMER_TABLE_NAME;

            request.Item = new Dictionary<string, AttributeValue>();

            request.Item.Add(CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id));
            request.Item.Add(CUSTOMER_NAME_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerName.ToLower()));
            request.Item.Add(DynamoUtilities.SERIALIZED_DATA_COLUMN, DynamoUtilities.GetItemAttributeSerializedValue(item));

            return request;
        }

        private static DeleteItemRequest CreateDeleteItemRequest(CustomerInfo item)
        {
            DeleteItemRequest request = new DeleteItemRequest();

            request.TableName = CUSTOMER_TABLE_NAME;
            request.Key = new Dictionary<string, AttributeValue>()
            { 
              {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)}
            };

            return request;
        }

        private ScanRequest CreateScanRequest(CustomerInfo searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria.CustomerName))
            {
                string errorMessage = string.Format(ErrorMessages.MissingRequiredAttribute, "Customer Name");
                throw new MissingFieldException(errorMessage);
            }

            ScanRequest request = new ScanRequest();

            request.TableName = CUSTOMER_TABLE_NAME;
            request.ScanFilter = new Dictionary<string, Condition>
            {
                {
                    CUSTOMER_NAME_COLUMN,
                    new Condition() 
                    {
                        ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR, 
                        AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.CustomerName.ToLower())
                        }
                    }
               }
            };

            return request;
        }
        #endregion
    }
}
