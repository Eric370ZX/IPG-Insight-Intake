using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class AddSecondCarrierTests : PluginTestsBase
    {
        [Fact]
        public void SetsSecondCarrier()
        {
            //ARRANGE
            var firstCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Commercial)
                    .Generate();
            var secondCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Commercial)
                    .Generate();
            var incident = new Incident().Fake()
                    .WithCarrierReference(firstCarrier)
                    .Generate();

            var caseCarrierAddingParameters = new ipg_CaseCarrierAddingParameters().Fake(incident, secondCarrier)
                    .Generate();
            
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", caseCarrierAddingParameters } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_CaseCarrierAddingParameters.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<AddSecondCarrier>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var incidentAfterUpdate = organizationService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.NotNull(incidentAfterUpdate.ipg_SecondaryCarrierId);
            Assert.Equal(secondCarrier.Id, incidentAfterUpdate.ipg_SecondaryCarrierId.Id);
            Assert.Equal(caseCarrierAddingParameters.ipg_MemberIdNumber, incidentAfterUpdate.ipg_SecondaryMemberIdNumber);
            Assert.Equal(caseCarrierAddingParameters.ipg_GroupNumber, incidentAfterUpdate.ipg_SecondaryCarrierGroupIdNumber);
        }

    }
}
