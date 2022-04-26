using FakeXrmEasy;
using Insight.Intake.Plugin.FacilityCarrierCPTRule;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.FacilityCarrierCPTRule
{
    public class UpdateFacilityCarrierCPTRuleNameTest : PluginTestsBase
    {
        [Fact]
        public void UpdateFacilityCarrierCPTRuleName()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;
            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            ipg_facilitycarriercptrule facilityCarrierCPTRule = new ipg_facilitycarriercptrule().Fake()
                                                                         .FakeWithCptCode(cptCode);

            OrganizationServiceMock.WithRetrieveCrud(cptCode);

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Create");

            var request = new CreateRequest
            {
                Target = facilityCarrierCPTRule.ToEntity<Entity>()
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(20);
            PluginExecutionContextMock.Setup(x => x.PrimaryEntityName).Returns(ipg_facilitycarriercptrule.EntityLogicalName);

            var outputParameters = new ParameterCollection
            {
                { "id", facilityCarrierCPTRule.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            Assert.True(string.IsNullOrEmpty(facilityCarrierCPTRule.ipg_name));
            var plugin = new UpdateFacilityCarrierCPTRuleName();
            plugin.Execute(ServiceProvider);
            Assert.Equal(facilityCarrierCPTRule.ipg_name, cptCode.ipg_cptcode1);

            #endregion
        }

    }
}
