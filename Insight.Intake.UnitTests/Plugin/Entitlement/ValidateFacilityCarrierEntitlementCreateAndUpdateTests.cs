using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Insight.Intake.Plugin.EntitlementNamespace;
using Xunit;
using Microsoft.Xrm.Sdk;
using Insight.Intake.UnitTests.Fakes;

namespace Insight.Intake.UnitTests.Plugin.EntitlementNamespace
{
    public class ValidateFacilityCarrierEntitlementCreateAndUpdateTests : PluginTestsBase
    {
        [Fact]
        public void ActivationNotAllowedForFacilityCarrierTest()
        {
            //ARRANGE

            Entitlement entitlement = new Entitlement().Fake(EntitlementState.Active)
                                     .WithEntitlementType(new OptionSetValue(
                                         (int)ipg_EntitlementTypes.FacilityCarrier));

            var inputParameters = new ParameterCollection
            {
                { "Target", entitlement.ToEntity<Entity>()}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            OrganizationServiceMock.WithRetrieveCrud(entitlement);

            ServiceProvider = ServiceProviderMock.Object;

            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Update);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(Entitlement.EntityLogicalName);


            //ACT

            var plugin = new ValidateFacilityCarrierEntitlementCreateAndUpdate();

            //ASSERT
            Assert.ThrowsException<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

        }
    }
}