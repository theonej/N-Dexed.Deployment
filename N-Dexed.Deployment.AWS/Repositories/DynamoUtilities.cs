using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using N_Dexed.Deployment.Common.Resources;
using Newtonsoft.Json;

namespace N_Dexed.Deployment.AWS.Repositories
{
    class DynamoUtilities
    {
        internal const string SERIALIZED_DATA_COLUMN = "SerializedData";

        internal static AmazonDynamoDBClient InitializeClient()
        {
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
            config.ServiceURL = ConfigurationManager.AppSettings["ServiceURL"];

            AmazonDynamoDBClient client = new AmazonDynamoDBClient(config);

            //other initialization actions here

            return client;
        }

        internal static AttributeValue GetItemAttributeStringValue(object item)
        {
            AttributeValue value = new AttributeValue();

            if (item != null)
                value.S = item.ToString();

            return value;
        }

        internal static AttributeValue GetItemAttributeSerializedValue(object item)
        {
            AttributeValue value = new AttributeValue();

            if (item != null)
                value.S = JsonConvert.SerializeObject(item);

            return value;
        }

        internal static T GetItemFromAttributeStore<T>(Dictionary<string, AttributeValue> attributeStore)
        {
            if (!attributeStore.ContainsKey(SERIALIZED_DATA_COLUMN))
            {
                string errorMessage = string.Format(ErrorMessages.MissingRequiredAttribute, SERIALIZED_DATA_COLUMN);
                throw new MissingFieldException(errorMessage);
            }
            string serializedValue = attributeStore[SERIALIZED_DATA_COLUMN].S;
            if (string.IsNullOrEmpty(serializedValue))
            {
                string errorMessage = string.Format(ErrorMessages.MissingRequiredAttribute, SERIALIZED_DATA_COLUMN);
                throw new DataMisalignedException(errorMessage);
            }

            T returnValue = JsonConvert.DeserializeObject<T>(serializedValue);

            return returnValue;
        }

    }
}
