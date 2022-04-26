using FakeXrmEasy;
using Insight.Intake.Plugin.EntitlementNamespace;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.UnitTests.Plugin.EntitlementNamespace
{
    public class SetBillToPatientTests : PluginTestsBase
    {
        [Fact]
        public void SetBillToPatientTests_Success()
        {
            var surgeryDate = new DateTime(2021, 1, 1);

            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier);
            Intake.Account facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility);

            Intake.Incident incident = new Intake.Incident()
                .Fake()
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .RuleFor(p => p.ipg_SurgeryDate, surgeryDate)
                .RuleFor(p => p.ipg_BillToPatient, new OptionSetValue((int)ipg_TwoOptions.Yes));

            Intake.Entitlement entitlement = new Intake.Entitlement()
                .Fake()
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEntitlementType(new OptionSetValue((int)ipg_EntitlementTypes.FacilityCarrier))
                .RuleFor(p => p.ipg_Billtopatient, true)
                .RuleFor(p => p.StartDate, surgeryDate.AddYears(-1))
                .RuleFor(p => p.EndDate, surgeryDate.AddYears(1));




            var listForInit = new List<Entity> { incident, carrier, facility, entitlement };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", entitlement } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Intake.Entitlement.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection() { { "PostImage", entitlement } },
            };
            fakedContext.ExecutePluginWith<SetBillToPatient>(pluginContext);


            var resultIncidents = fakedContext.CreateQuery(Incident.EntityLogicalName);
            Assert.Equal(resultIncidents.FirstOrDefault().ToEntity<Incident>().ipg_BillToPatient.Value, (int)ipg_TwoOptions.Yes);
        }
    }
}
