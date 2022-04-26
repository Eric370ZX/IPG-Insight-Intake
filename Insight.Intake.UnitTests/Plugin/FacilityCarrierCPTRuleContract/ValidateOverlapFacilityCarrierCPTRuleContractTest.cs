using Insight.Intake.Plugin.FacilityCarrierCPTRule;
using Insight.Intake.Plugin.FacilityCarrierCPTRuleContract;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.FacilityCarrierCPTRuleContract
{
    public class ValidateOverlapFacilityCarrierCPTRuleContractTest : PluginTestsBase
    {
        [Fact]
        public void ValidateUpdateFacilityCPTRuleWithOverlapping()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;
            Entitlement carrierFacilityRelationship = new Entitlement().Fake();
            carrierFacilityRelationship.StartDate = DateTime.Today;
            carrierFacilityRelationship.EndDate = DateTime.Today;

            ipg_facilitycarriercptrulecontract facilitycarriercptrulecontract =
                new ipg_facilitycarriercptrulecontract().Fake()
                                                .FakeWithEntitlement(carrierFacilityRelationship);

            OrganizationServiceMock.WithRetrieveCrud(facilitycarriercptrulecontract);

            var opposingRules = new ipg_facilitycarriercptrulecontract().Fake().Generate(1);
            OrganizationServiceMock.WithRetrieveMultipleCrud(
                 ipg_facilitycarriercptrulecontract.EntityLogicalName,
                 opposingRules.Cast<Entity>().ToList()
             );

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            var request = new ParameterCollection
            {
                 { "Target", facilitycarriercptrulecontract.ToEntity<Entity>()}
            };

            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(request);
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Update);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_facilitycarriercptrulecontract.EntityLogicalName);

            var outputParameters = new ParameterCollection
            {
                { "id", facilitycarriercptrulecontract.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new ValidateOverlapFacilityCarrierCPTRuleContract();
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

            #endregion
        }

    }
}
