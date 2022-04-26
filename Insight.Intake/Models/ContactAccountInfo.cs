using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Insight.Intake.Models
{
    public class ContactAccountInfo
    {
        [DataMember]
        public Guid id { get; set; }
        [DataMember]
        public List<ipg_contactsaccounts_ipg_ContactRoleCode> Roles { get; set; }
    }
}
