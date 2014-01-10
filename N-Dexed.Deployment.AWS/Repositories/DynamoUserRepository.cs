using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using N_Dexed.Deployment.Common.Domain.Customer;
using N_Dexed.Deployment.Common.Domain.Messaging;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;

namespace N_Dexed.Deployment.AWS.Repositories
{
    public class DynamoUserRepository : IRepository<UserInfo>
    {
        private readonly IMessageLogger m_Logger;

        public DynamoUserRepository(IMessageLogger logger)
        {
            m_Logger = logger;
        }

        private const string USER_TABLE_NAME = "n-dexed.deployment.users";
        private const string USER_ID_COLUMN = "UserId";
        private const string CUSTOMER_ID_COLUMN = "CustomerId";
        private const string USER_NAME_COLUMN = "UserName";
        private const string EMAIL_ADDRESS_COLUMN = "EmailAddress";
        private const string PASSWORD_COLUMN = "Password";

        public UserInfo Get(UserInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                GetItemRequest request = CreateGetItemRequest(item);

                GetItemResponse response = client.GetItem(request);

                if (response.Item == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Get User");
                    throw new MissingFieldException(errorMessage);
                }

                UserInfo returnValue = DynamoUtilities.GetItemFromAttributeStore<UserInfo>(response.Item);

                return returnValue;
            }
        }

        public Guid Save(UserInfo item)
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

        /// <summary>
        /// This method will use whatever information is supplied to scan the table
        /// supported fields are:
        /// CustomerId
        /// UserName
        /// Password(Hash)
        /// EmailAddress
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public List<UserInfo> Search(UserInfo searchCriteria)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                ScanRequest request = CreateScanRequest(searchCriteria);

                ScanResponse response = client.Scan(request);

                if (response.Items == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Search User");
                    throw new MissingFieldException(errorMessage);
                }

                List<UserInfo> returnValue = new List<UserInfo>();

                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    UserInfo user = DynamoUtilities.GetItemFromAttributeStore<UserInfo>(item);
                    returnValue.Add(user);
                }

                return returnValue;
            }

            throw new NotImplementedException();
        }

        public void Delete(UserInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                DeleteItemRequest request = CreateDeleteItemRequest(item);

                client.DeleteItem(request);
            }
        }

        #region Private Methods

        private static PutItemRequest CreatePutItemRequest(UserInfo item)
        {
            PutItemRequest request = new PutItemRequest();

            request.TableName = USER_TABLE_NAME;

            request.Item = new Dictionary<string, AttributeValue>();

            request.Item.Add(USER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id));
            request.Item.Add(CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId));
            request.Item.Add(EMAIL_ADDRESS_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.EmailAddress));
            request.Item.Add(USER_NAME_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.UserName));
            request.Item.Add(PASSWORD_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.PasswordHash));
            request.Item.Add(DynamoUtilities.SERIALIZED_DATA_COLUMN, DynamoUtilities.GetItemAttributeSerializedValue(item));

            return request;
        }

        private static DeleteItemRequest CreateDeleteItemRequest(UserInfo item)
        {
            DeleteItemRequest request = new DeleteItemRequest();

            request.TableName = USER_TABLE_NAME;

            request.Key = new Dictionary<string, AttributeValue>
            {
                {USER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)},
                {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId)}
            };

            return request;
        }

        private static GetItemRequest CreateGetItemRequest(UserInfo item)
        {
            GetItemRequest request = new GetItemRequest();

            request.TableName = USER_TABLE_NAME;

            request.Key = new Dictionary<string, AttributeValue>
            {
                {USER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)},
                {CUSTOMER_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.CustomerId)}
            };

            return request;
        }

        private static ScanRequest CreateScanRequest(UserInfo searchCriteria)
        {
            ScanRequest request = new ScanRequest();

            request.TableName = USER_TABLE_NAME;

            request.ScanFilter = new Dictionary<string, Condition>();

            if (searchCriteria.CustomerId != Guid.Empty)
            {
                Condition condition = new Condition();
                condition.ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR;
                condition.AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.CustomerId)
                        };

                request.ScanFilter.Add(CUSTOMER_ID_COLUMN, condition);
            }

            if (! string.IsNullOrEmpty(searchCriteria.EmailAddress))
            {
                Condition condition = new Condition();
                condition.ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR;
                condition.AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.EmailAddress)
                        };

                request.ScanFilter.Add(EMAIL_ADDRESS_COLUMN, condition);
            }

            if (!string.IsNullOrEmpty(searchCriteria.PasswordHash))
            {
                Condition condition = new Condition();
                condition.ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR;
                condition.AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.PasswordHash)
                        };

                request.ScanFilter.Add(PASSWORD_COLUMN, condition);
            }

            if (!string.IsNullOrEmpty(searchCriteria.UserName))
            {
                Condition condition = new Condition();
                condition.ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR;
                condition.AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.UserName)
                        };

                request.ScanFilter.Add(USER_NAME_COLUMN, condition);
            }

            if (searchCriteria.Id != Guid.Empty)
            {
                Condition condition = new Condition();
                condition.ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR;
                condition.AttributeValueList = new List<AttributeValue>()
                        {
                            DynamoUtilities.GetItemAttributeStringValue(searchCriteria.Id)
                        };

                request.ScanFilter.Add(USER_ID_COLUMN, condition);
            }

            return request;
        }

        #endregion
    }
}
