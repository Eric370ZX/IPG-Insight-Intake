using System;

namespace Insight.Intake.Models
{
    public class EligibilityInquiry
    {
        public string id { get; set; } //lower case because Cosmos DB needs it
        public int RequestId { get; set; }
        public string RequestedBy { get; set; }
        public string CaseId { get; set; }
        public string CarrierId { get; set; }
        public string PatientId { get; set; }
        public bool IsUserGenerated { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Status { get; set; } /*See Statuses nested class*/
        public string EligibilityId { get; set; }


        //HIPPA X12 207 technical fields

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }


        //inquiry data

        public string PayerName { get; set; }
        public string PayerCode { get; set; }

        public string ProviderName { get; set; }
        public string ProviderNpi { get; set; }
        public string ProviderTaxId { get; set; }

        public string MemberId { get; set; }
        public string PolicyNumber { get; set; }
        public string RelationToInsured { get; set; }

        public string SubscriberLastName { get; set; }
        public string SubscriberFirstName { get; set; }
        public string SubscriberMiddleName { get; set; }

        public DateTime? SubscriberDOB { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public bool SubscriberDOBSpecified { get { return this.SubscriberDOB != null; } }

        public string SubscriberGender { get; set; } //string type bacause XmlSerializer does not support char

        public string DependentLastName { get; set; }
        public string DependentFirstName { get; set; }
        public string DependentMiddleName { get; set; }

        public DateTime? DependentDOB { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public bool DependentDOBSpecified { get { return this.DependentDOB != null; } }

        public string DependentGender { get; set; } //string type bacause XmlSerializer does not support char

        public DateTime? DOS { get; set; }

        public class Statuses
        {
            public static readonly string INPROCESS = "INPROCESS";
            public static readonly string COMPLETE = "COMPLETE";
            public static readonly string SERVER_DOWN = "SERVER_DOWN";
            public static readonly string ERROR = "ERROR";
        }
    }
}
