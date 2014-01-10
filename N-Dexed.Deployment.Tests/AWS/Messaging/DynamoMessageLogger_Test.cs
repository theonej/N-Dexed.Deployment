using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.AWS.Messaging;
using N_Dexed.Deployment.Common.Domain.Messaging;

namespace N_Dexed.Deployment.Tests.AWS.Messaging
{
    [TestClass]
    public class DynamoMessageLogger_Test
    {
        [TestMethod]
        public void LogMessage()
        {
            DynamoMessageLogger logger = new DynamoMessageLogger();

            MessageInfo message = new MessageInfo();
            message.Message = "Test Message";
            message.MessageType = MessageTypes.Info;
            message.Timestamp = DateTime.Now;

            logger.WriteMessage(message);
        }

        [TestMethod]
        public void LogError()
        {
            DynamoMessageLogger logger = new DynamoMessageLogger();

            ErrorInfo error = new ErrorInfo();
     
            error.ErrorCode = "TestError400";
            error.Message = "A test Error";

            logger.WriteError(error);
        }
    }
}
