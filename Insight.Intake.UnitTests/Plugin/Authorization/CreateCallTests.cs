using FakeXrmEasy;
using Insight.Intake.Plugin.Authorization;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Authorization
{
    public class CreateCallTests
    {
        [Fact]
        public void CreateCallTest()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            ipg_authorization target = new ipg_authorization()
                .Fake()
                    .WithPhoneReference("phone-ref-666")
                    .WithPhoneNumber("222-45-45")
                    .WithIncidentRef(incident.ToEntityReference())
                .Generate();

            var fakedEntities = new List<Entity>() { incident, target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateCall>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                PhoneCall phoneCall = context.PhoneCallSet.FirstOrDefault(x => x.RegardingObjectId.Id == incident.Id);

                Assert.NotNull(phoneCall);
                Assert.NotNull(target.ipg_PhoneCallId);
                Assert.Equal(phoneCall.Subject, target.ipg_callreference);
                Assert.Equal(target.ipg_PhoneCallId.Id, phoneCall.Id);
            }
        }
    }
}
