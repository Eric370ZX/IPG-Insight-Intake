using Insight.Intake.Data;
using Insight.Intake.Plugin.FacilityCPT;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.FacilityCPT
{
    public class OnFacilityCPTCreateAndUpdateTest : PluginTestsBase
    {
        [Fact]
        public void ValidateUpdateDuplicateFacilityCPT()
        {
            #region Setup services

            ServiceProvider = ServiceProviderMock.Object;
 
            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);
            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            ipg_facilitycpt facilitycpt = new ipg_facilitycpt().Fake().WitFacility(facility)
                                        .WithCPT(cptCode);
            facilitycpt.ipg_EffectiveDate = DateTime.Today;
            facilitycpt.ipg_ExpirationDate = DateTime.Today;

            OrganizationServiceMock.WithRetrieveCrud(facilitycpt);

            Intake.Account facility1 = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);
            ipg_cptcode cptCode1 = new ipg_cptcode().Fake();

            var duplicateFacilityCPT = new ipg_facilitycpt().Fake().WitFacility(facility)
                                        .WithCPT(cptCode).Generate(1);
            OrganizationServiceMock.WithRetrieveMultipleCrud(
                 ipg_facilitycpt.EntityLogicalName,
                 duplicateFacilityCPT.Cast<Entity>().ToList()
             );

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup execution context
            var request = new ParameterCollection
            {
                 { "Target", facilitycpt.ToEntity<Entity>()}
            };

            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(request);
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Update);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_facilitycpt.EntityLogicalName);

            var outputParameters = new ParameterCollection
            {
                { "id", facilitycpt.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new OnFacilityCPTCreateAndUpdate();
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

            #endregion
        }
    }
}
