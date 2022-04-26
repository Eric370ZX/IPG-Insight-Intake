using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Managers
{
    public class BvfManager
    {
        private IOrganizationService crmService;
        private ITracingService traceService;

        public BvfManager(IOrganizationService crmService, ITracingService traceService)
        {
            this.crmService = crmService;
            this.traceService = traceService;
        }

        public IEnumerable<BvfPdfMap> GetMappedFields(Entity bvfEntity, DateTime localDateTime, TimeSpan diffTime)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = "";
            var result = new List<BvfPdfMap>() {
               new BvfPdfMap("Authorization Specialist",bvfEntity.GetAttributeValue<EntityReference>("ipg_authorizationspecialistnameid")?.Name),
                new BvfPdfMap("Facility",bvfEntity.GetAttributeValue<string>("ipg_facility")),
                new BvfPdfMap("Procedure",bvfEntity.GetAttributeValue<string>("ipg_procedurename")),
                new BvfPdfMap("Surgeon",bvfEntity.GetAttributeValue<EntityReference>("ipg_physicianid")?.Name),
                new BvfPdfMap("DOS", PrepareDateString(bvfEntity, "ipg_surgerydate", diffTime)),
                new BvfPdfMap("Patient Name",$"{bvfEntity.GetAttributeValue<string>("ipg_patientfirstname")}, {bvfEntity.GetAttributeValue<string>("ipg_patientlastname")}"),
                new BvfPdfMap("Insight ID",bvfEntity.GetAttributeValue<string>("ipg_caseid")),
                new BvfPdfMap("DOB",PrepareDateString(bvfEntity, "ipg_patientdateofbirth", diffTime)),
                new BvfPdfMap("Date",localDateTime.ToShortDateString()),
                new BvfPdfMap("Time",localDateTime.ToShortTimeString()),
                new BvfPdfMap("CPT Code1",bvfEntity.GetAttributeValue<string>("ipg_cptcode1")),
                new BvfPdfMap("CPT Code2",bvfEntity.GetAttributeValue<string>("ipg_cptcode2")),
                new BvfPdfMap("CPT Code3",$"{bvfEntity.GetAttributeValue<string>("ipg_cptcode3")}, {bvfEntity.GetAttributeValue<string>("ipg_cptcode4")}, {bvfEntity.GetAttributeValue<string>("ipg_cptcode5")}"),
                new BvfPdfMap("Policy Holder Name",bvfEntity.GetAttributeValue<string>("ipg_policyholdername")),
                new BvfPdfMap("Insurance Company",bvfEntity.GetAttributeValue<EntityReference>("ipg_carrierid")?.Name),
                new BvfPdfMap("Home Plan Insurance Company",bvfEntity.GetAttributeValue<EntityReference>("ipg_homeplancarrierid")?.Name),
                new BvfPdfMap("Policy ID",bvfEntity.GetAttributeValue<string>("ipg_memberidnumber")),//populate from some field
                new BvfPdfMap("Group",bvfEntity.GetAttributeValue<string>("ipg_primarycarriergroupidnumber")),
                new BvfPdfMap("Plan Type",bvfEntity.FormattedValues.Contains("ipg_primarycarrierplantype")?bvfEntity.FormattedValues["ipg_primarycarrierplantype"]:""),
                new BvfPdfMap("Effective Date",$"{PrepareDateString(bvfEntity, "ipg_coverageeffectivedate", diffTime)} - {PrepareDateString(bvfEntity, "ipg_coverageexpirationdate", diffTime)}"),
                new BvfPdfMap("Ded",$"{bvfEntity.GetAttributeValue<Money>("ipg_deductible")?.Value}"),
                new BvfPdfMap("DedMet",$"{bvfEntity.GetAttributeValue<Money>("ipg_deductiblemet")?.Value}"),
                new BvfPdfMap("InsCoPay",$"{bvfEntity.GetAttributeValue<decimal?>("ipg_carriercoinsurance")}"),
                new BvfPdfMap("PtCoPay",$"{bvfEntity.GetAttributeValue<decimal?>("ipg_patientcoinsurance")}"),
                new BvfPdfMap("OopMax",$"{bvfEntity.GetAttributeValue<Money>("ipg_oopmax")?.Value.ToString("N0",nfi)}"),
                new BvfPdfMap("OOP Met",$"{bvfEntity.GetAttributeValue<Money>("ipg_oopmaxmet")?.Value.ToString("N0")}"),
                new BvfPdfMap("Ded",$"{bvfEntity.GetAttributeValue<Money>("ipg_oopmax")?.Value.ToString("N0")}"),
                new BvfPdfMap("DedMet",$"{bvfEntity.GetAttributeValue<Money>("ipg_oopmaxmet")?.Value.ToString("N0")}"),
                new BvfPdfMap("Authorization number",$"{bvfEntity.GetAttributeValue<string>("ipg_ipgauth")}"),
                new BvfPdfMap("start",$"{PrepareDateString(bvfEntity, "ipg_autheffectivedate", diffTime)}"),
                new BvfPdfMap("end",$"{PrepareDateString(bvfEntity, "ipg_authexpirationdate", diffTime)}"),
                new BvfPdfMap("Name",$"{bvfEntity.GetAttributeValue<string>("ipg_csrnamebenefit")}"),
                new BvfPdfMap("Benefits ph",$"{bvfEntity.GetAttributeValue<string>("ipg_csrphonebenefits")}"),
                new BvfPdfMap("CallRefNum",$"{bvfEntity.GetAttributeValue<string>("ipg_callreferencebenefits")}"),
                new BvfPdfMap("Claims Mailing Address",$"{bvfEntity.GetAttributeValue<string>("ipg_claimsmailingaddress")}"),
                new BvfPdfMap("Rep Name",$"{bvfEntity.GetAttributeValue<string>("ipg_csrname")}"),
                new BvfPdfMap("Phone",$"{bvfEntity.GetAttributeValue<string>("ipg_csrphone")}"),
                new BvfPdfMap("Call Ref",$"{bvfEntity.GetAttributeValue<string>("ipg_callreference")}"),
                new BvfPdfMap("Notesor other codes not listed above",$"{bvfEntity.GetAttributeValue<string>("ipg_authorizationnotes")}\n{bvfEntity.GetAttributeValue<string>("ipg_benefitnotesmultiplelines")}"),

            };
            return result;
        }
        private string PrepareDateString(Entity bvfEntity, string datetimeString, TimeSpan diffTime)
        {
            string result = null;
            var datetime = bvfEntity.GetAttributeValue<DateTime?>(datetimeString);
            if (datetime != null)
            {
                result = ((DateTime)datetime + diffTime).ToShortDateString();
            }
            return result;
        }
    }
    public class BvfPdfMap
    {
        public BvfPdfMap(string pdfField, string value)
        {
            PdfField = pdfField;
            Value = value;
        }
        public string CrmName { get; set; }
        public string PdfField { get; set; }
        public string Value { get; set; }
    }
}
