using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class SetRelatedToInsured : PluginBase
    {
        public SetRelatedToInsured() : base(typeof(SetRelatedToInsured))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetIncident = localPluginContext.Target<Incident>();

            if (targetIncident.ipg_PatientId == null)
            {
                return;
            }

            var patient = localPluginContext.SystemOrganizationService
                .Retrieve(Intake.Contact.EntityLogicalName, targetIncident.ipg_PatientId.Id, new ColumnSet(true))
                .ToEntity<Intake.Contact>();

            targetIncident.ipg_RelationToInsuredEnum = new_RelationtoInsured.Self;
            targetIncident.ipg_InsuredFirstName = patient.FirstName;
            targetIncident.ipg_InsuredLastName = patient.LastName;
            targetIncident.ipg_InsuredMiddleName = patient.MiddleName;
            targetIncident.ipg_InsuredGenderEnum =GenderConvert(patient.GenderCodeEnum);
            targetIncident.ipg_InsuredDateOfBirth = patient.BirthDate;
            targetIncident.ipg_insuredaddress = patient.Address1_Line1;
            targetIncident.ipg_insuredcity = patient.Address1_City;
            targetIncident.ipg_insuredstate = patient.Address1_StateOrProvince;
            // targetIncident.ipg_insuredzipcodeid = patient.ipg_zipcodeid;
            targetIncident.ipg_caseinsuredzipcodeId = patient.ipg_melissazipcodeid;
            var insuredPhone = GetInsuredPhone(patient);
            targetIncident.ipg_insuredphone = insuredPhone?.Phone;
            targetIncident.ipg_insuredphonetypeEnum = insuredPhone?.PhoneType;


        }

        private InsuredPhone GetInsuredPhone(Intake.Contact patient)
        {
            if (!string.IsNullOrEmpty(patient.MobilePhone))
            {
                return new InsuredPhone() {Phone= patient.MobilePhone,PhoneType=ipg_PhoneType.Cell };
            }
            if (!string.IsNullOrEmpty(patient.Address1_Telephone1))
            {
                return new InsuredPhone() { Phone = patient.Address1_Telephone1, PhoneType = ipg_PhoneType.Home };
            }
            if (!string.IsNullOrEmpty(patient.Address1_Telephone2))
            {
                return new InsuredPhone() { Phone = patient.Address1_Telephone2, PhoneType = ipg_PhoneType.Work };
            }
            return null;
        }

        private ipg_Gender? GenderConvert(Contact_GenderCode? genderCode)
        {
            if (genderCode == null)
            {
                return null;
            }
            switch (genderCode)
            {
                case Contact_GenderCode.Male:
                    return ipg_Gender.Male;
                case Contact_GenderCode.Female:
                    return ipg_Gender.Female;

                default:
                    return ipg_Gender.Other;
            }
        }

        class InsuredPhone
        {
            public ipg_PhoneType PhoneType { get; set; }
            public string Phone { get; set; }
        }
    }

}
