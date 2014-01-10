
using N_Dexed.Deployment.Common.Domain.Messaging;

namespace N_Dexed.Deployment.AWS.Messaging
{
    public class DynamoMessageLogger : IMessageLogger
    {
        public void WriteMessage(MessageInfo message)
        {
            //This might be a good place for Caleb to start
        }

        public void WriteError(ErrorInfo error)
        {
           //we want to inject dynamo repositories for errors and for messagesd and have this class call them with the appropriate methods.

            //walk through table creation and repository creation for messages to show methodology
        }


        public void WriteException(System.Exception exception)
        {
            //convert the exception to an error info object, then log
        }
    }
}
