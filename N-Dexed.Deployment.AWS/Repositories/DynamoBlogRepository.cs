using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using N_Dexed.Deployment.Common.Domain.Blog;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.AWS.Repositories
{
    public class DynamoBlogRepository : IRepository<BlogInfo>
    {
        private const string BLOG_TABLE_NAME = "n-dexed.deployment.blogs";
        private const string BLOG_ID_COLUMN = "BlogId";

        public BlogInfo Get(BlogInfo item)
        {
            throw new NotImplementedException();
        }

        public Guid Save(BlogInfo item)
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

        public List<BlogInfo> Search(BlogInfo searchCriteria)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using(client)
            {
                ScanRequest scanRequest = CreateScanRequest(searchCriteria);

                ScanResponse response = client.Scan(scanRequest);

                if (response.Items == null)
                {
                    string errorMessage = string.Format(ErrorMessages.MisingResponseItem, "Search Blogs");
                    throw new MissingFieldException(errorMessage);
                }

                List<BlogInfo> returnValue = new List<BlogInfo>();
                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    BlogInfo customer = DynamoUtilities.GetItemFromAttributeStore<BlogInfo>(item);
                    returnValue.Add(customer);
                }

                return returnValue;
            }
        }

        public void Delete(BlogInfo item)
        {
            AmazonDynamoDBClient client = DynamoUtilities.InitializeClient();
            using (client)
            {
                DeleteItemRequest request = CreateDeleteItemRequest(item);

                client.DeleteItem(request);
            }
        }

        #region Private Methods

        private ScanRequest CreateScanRequest(BlogInfo searchCriteria)
        {
            ScanRequest request = new ScanRequest();

            request.TableName = BLOG_TABLE_NAME;
            request.ScanFilter = new Dictionary<string, Condition>();
            if (searchCriteria.Id != Guid.Empty)
            {
                Condition customerIdCondition = new Condition()
                {
                    ComparisonOperator = Constants.DYNAMO_EQUALITY_OPERATOR,
                    AttributeValueList = new List<AttributeValue>()
                            {
                                DynamoUtilities.GetItemAttributeStringValue(searchCriteria.Id)
                            }
                };

                request.ScanFilter.Add(BLOG_ID_COLUMN, customerIdCondition);
            }
            return request;
        }

        private PutItemRequest CreatePutItemRequest(BlogInfo item)
        {
            PutItemRequest request = new PutItemRequest();

            request.TableName = BLOG_TABLE_NAME;

            request.Item = new Dictionary<string, AttributeValue>();

            request.Item.Add(BLOG_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id));
            request.Item.Add(DynamoUtilities.SERIALIZED_DATA_COLUMN, DynamoUtilities.GetItemAttributeSerializedValue(item));

            return request;
        }

        private static DeleteItemRequest CreateDeleteItemRequest(BlogInfo item)
        {
            DeleteItemRequest request = new DeleteItemRequest();

            request.TableName = BLOG_TABLE_NAME;
            request.Key = new Dictionary<string, AttributeValue>()
            { 
              {BLOG_ID_COLUMN, DynamoUtilities.GetItemAttributeStringValue(item.Id)}
            };

            return request;
        }

        #endregion
    }
}
