using Insight.Intake.Plugin.FacilityCarrierCPTRule;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.FacilityCarrierCPTRule
{
    public class ValidateOverlappingCPTRulesTest : PluginTestsBase
    {
        [Fact]
        public void ValidateUpdateFacilityCPTRuleWithOverlapping()
         {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;
            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            ipg_facilitycarriercptrulecontract contract = new ipg_facilitycarriercptrulecontract().Fake()
                .WithEffectivesDates(DateTime.Today, DateTime.Today);

            ipg_facilitycarriercptrule facilityCarrierCPTRule = 
                new ipg_facilitycarriercptrule().Fake()
                                                .FakeWithCptCode(cptCode)
                                                .FakeWithContract(contract);

            OrganizationServiceMock.WithRetrieveCrud(facilityCarrierCPTRule);

            var opposingRules = new ipg_facilitycarriercptrule().Fake().Generate(1);
            OrganizationServiceMock.WithRetrieveMultipleCrud(
                 ipg_facilitycarriercptrule.EntityLogicalName,
                 opposingRules.Cast<Entity>().ToList()
             );

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            var request = new ParameterCollection
            {
                 { "Target", facilityCarrierCPTRule.ToEntity<Entity>()}
            };

            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(request);
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Update);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_facilitycarriercptrule.EntityLogicalName);

            var outputParameters = new ParameterCollection
            {
                { "id", facilityCarrierCPTRule.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new ValidateOverlappingCPTRules();
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

            #endregion
        }

    }
}
