using FakeXrmEasy;
using Insight.Intake.Plugin.Account;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Insight.Intake.UnitTests.Plugin.Account
{
    public class SetBillToPatientTests : PluginTestsBase
    {
        [Fact]
        public void SetBillToPatientTests_Success()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier);
            carrier.ipg_StatementToPatient = true;

            Intake.Incident incident = new Intake.Incident()
                .Fake()
                .WithCarrierReference(carrier)
                .RuleFor(p=>p.ipg_BillToPatient, new OptionSetValue((int)ipg_TwoOptions.Yes));
            

            var listForInit = new List<Entity> { incident, carrier };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection() { { "PostImage", carrier } },
            };
            fakedContext.ExecutePluginWith<SetBillToPatient>(pluginContext);


            var resultIncidents = fakedContext.CreateQuery(Incident.EntityLogicalName);
            Assert.Equal(resultIncidents.FirstOrDefault().ToEntity<Incident>().ipg_BillToPatient.Value, (int)ipg_TwoOptions.Yes);
        }
    }
}
