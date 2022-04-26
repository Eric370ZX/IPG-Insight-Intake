using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Insight.Intake.Models
{
    [DataContract]
    public class PortalContactBackUpJson
    {
        [DataMember]
        public List<ContactAccountInfo> facilities { get; set; }
        [DataMember]
        public List<Guid> WebRoles { get; set; }
    }
}
