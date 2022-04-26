using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Authorization;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Authorization
{
    public class CaseRecentAuthorizationTests
    {
        [Fact]
        public void RecentAuthForPrimaryCarrierTest()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .Generate();

            Incident incident = new Incident()
                .Fake()
                    .WithPrimaryCarrierReference(carrier)
                .Generate();

            ipg_authorization target = new ipg_authorization()
                .Fake()
                    .WithPhoneReference("phone-ref-666")
                    .WithPhoneNumber("222-45-45")
                    .WithIncidentRef(incident.ToEntityReference())
                    .WithCarrier(carrier.ToEntityReference())
                .Generate();

            var fakedEntities = new List<Entity>() { carrier, incident, target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CaseRecentAuthorization>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                incident = context.IncidentSet.FirstOrDefault(x => x.Id == incident.Id);

                Assert.NotNull(incident);
                Assert.NotNull(incident.ipg_AuthorizationId);
                Assert.Equal(incident.ipg_AuthorizationId.Id, target.Id);
            }
        }

        [Fact]
        public void RecentAuthForSecondaryCarrierTest()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account primaryCarrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .Generate();

            Intake.Account secondaryCarrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .Generate();

            Incident incident = new Incident()
                .Fake()
                    .WithPrimaryCarrierReference(primaryCarrier)
                    .WithCarrierReference(secondaryCarrier, false)
                .Generate();

            ipg_authorization target = new ipg_authorization()
                .Fake()
                    .WithPhoneReference("phone-ref-666")
                    .WithPhoneNumber("222-45-45")
                    .WithIncidentRef(incident.ToEntityReference())
                    .WithCarrier(secondaryCarrier.ToEntityReference())
                .Generate();

            var fakedEntities = new List<Entity>() { primaryCarrier, incident, target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CaseRecentAuthorization>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                incident = context.IncidentSet.FirstOrDefault(x => x.Id == incident.Id);

                Assert.NotNull(incident);
                Assert.Null(incident.ipg_AuthorizationId);
                Assert.NotNull(incident.ipg_secondaryauthorizationid);
                Assert.Equal(incident.ipg_secondaryauthorizationid.Id, target.Id);
            }
        }
    }
}
