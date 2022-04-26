using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;
using System;
using Insight.Intake.Data;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CPTSupportedByIPGTests : PluginTestsBase
    {
        [Fact]
        public void CPTSupportedByIPGForImplantUsed()
        {


            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;
            ipg_referral referral = new ipg_referral().Fake();
            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            cptCode.ipg_ImplantUsed = new OptionSetValue((int)ipg_cptcode_ipg_ImplantUsed.Yes);
            cptCode.StateCode = ipg_cptcodeState.Active;
            referral.ipg_CPTCodeId1 = cptCode.ToEntityReference();

            referral.ipg_SurgeryDate = new DateTime(2019, 11, 12);
            OrganizationServiceMock.WithRetrieveCrud(cptCode);
            OrganizationServiceMock.WithRetrieveCrud(referral);

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup execution context


            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGGatingCPTSupportedByIPG");

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(inputParameters);
            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(PipelineStages.PostOperation);
          
            #endregion

            #region Execute plugin
            var plugin = new CPTSupportedByIPG();
            plugin.Execute(ServiceProvider);
            Assert.True(((bool)PluginExecutionContextMock.Object.OutputParameters["Succeeded"]));
            Assert.Empty(((string)PluginExecutionContextMock.Object.OutputParameters["CaseNote"]));
            Assert.Empty(((string)PluginExecutionContextMock.Object.OutputParameters["PortalNote"]));

            #endregion
        }

        [Fact]
        public void CPTSupportedByIPGForNoImplantUsed()
        {


            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;
            ipg_referral referral = new ipg_referral().Fake();
            ipg_cptcode cptCode = new ipg_cptcode().Fake();

            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);
            cptCode.ipg_ImplantUsed = new OptionSetValue((int)ipg_cptcode_ipg_ImplantUsed.No);
            cptCode.StateCode = ipg_cptcodeState.Active;
            referral.ipg_CPTCodeId1 = cptCode.ToEntityReference();
            referral.ipg_FacilityId = facility.ToEntityReference();

            referral.ipg_SurgeryDate = new DateTime(2019, 11, 12);
            OrganizationServiceMock.WithRetrieveCrud(cptCode);
            OrganizationServiceMock.WithRetrieveCrud(referral);
            OrganizationServiceMock.WithRetrieveCrud(facility);
            OrganizationServiceMock.WithRetrieveCrud(new SystemUser()
            {
                FirstName = "test",
                LastName = "test"
            });

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup execution context


            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGGatingCPTSupportedByIPG");

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(inputParameters);
            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(PipelineStages.PostOperation);

            #endregion

            #region Execute plugin
            var plugin = new CPTSupportedByIPG();
            plugin.Execute(ServiceProvider);
            Assert.False(((bool)PluginExecutionContextMock.Object.OutputParameters["Succeeded"]));
            Assert.NotEmpty(((string)PluginExecutionContextMock.Object.OutputParameters["CaseNote"]));
            Assert.NotEmpty(((string)PluginExecutionContextMock.Object.OutputParameters["PortalNote"]));




            #endregion
        }
    }
}