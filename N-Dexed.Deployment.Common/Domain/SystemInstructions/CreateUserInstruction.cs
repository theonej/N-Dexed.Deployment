using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using N_Dexed.Deployment.Common.Domain.SystemInstructions;

namespace N_Dexed.Deployment.Common.SystemInstructions
{
    [DataContract]
    public class CreateUserInstruction : ISystemInstruction
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
    }
}
