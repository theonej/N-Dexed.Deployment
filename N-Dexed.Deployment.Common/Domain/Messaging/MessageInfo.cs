using System;

namespace N_Dexed.Deployment.Common.Domain.Messaging
{
    public enum MessageTypes
    {
        Info = 0,
        Debug = 1,
        Error = 2,
        Critical = 3
    };

    public class MessageInfo
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageTypes MessageType { get; set; }
    }
}
