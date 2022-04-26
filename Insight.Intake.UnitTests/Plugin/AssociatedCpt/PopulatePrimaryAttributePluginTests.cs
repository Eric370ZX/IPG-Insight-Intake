using System;
using Insight.Intake.Plugin.AssociatedCpt;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.AssociatedCpt
{
    public class PopulatePrimaryAttributePluginTests : PluginTestsBase
    {
        [Fact]
        public void CheckWhatAssociatedCptPrimaryAttributeIsGeneratedCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;
            
            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            
            Intake.Account facility = new Intake.Account().Fake();

            Intake.Account carrier = new Intake.Account().Fake();

            ipg_associatedcpt createdRecord = new ipg_associatedcpt().Fake()
                .WithCptCodeReference(cptCode)
                //.WithFacilityReference(facility)
                .WithCarrierReference(carrier);

            OrganizationServiceMock.WithRetrieveCrud(cptCode);

            OrganizationServiceMock.WithRetrieveCrud(facility);

            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Create");

            var request = new CreateRequest
            {
                Target = createdRecord.ToEntity<Entity>()
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection
            {
                { "id", Guid.NewGuid() }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new PopulatePrimaryAttributePlugin();

            plugin.Execute(ServiceProvider);

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            if (!pluginExecutionContext.InputParameters.Contains("Target") || !(pluginExecutionContext.InputParameters["Target"] is Entity))
            {
                throw new Exception("Input target should be Entity.");
            }

            var exptected = $"{carrier.Name} - {cptCode.ipg_name}";
            
            var record = ((Entity)pluginExecutionContext.InputParameters["Target"]).ToEntity<ipg_associatedcpt>();

            Assert.Equal(exptected, record.ipg_name);
            #endregion
        }

        [Fact]
        public void CheckWhatAssociatedCptPrimaryAttributeIsGeneratedCorrectlyAfterUpdate()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_associatedcpt createdRecord = new ipg_associatedcpt().Fake();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();

            Intake.Account facility = new Intake.Account().Fake();

            Intake.Account carrier = new Intake.Account().Fake();

            ipg_associatedcpt updatedRecord = new ipg_associatedcpt().Fake()
                .WithCptCodeReference(cptCode)
                //.WithFacilityReference(facility)
                .WithCarrierReference(carrier);

            OrganizationServiceMock.WithRetrieveCrud(cptCode);

            OrganizationServiceMock.WithRetrieveCrud(facility);

            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Update");

            var request = new CreateRequest
            {
                Target = createdRecord.ToEntity<Entity>()
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var imagesCollection = new EntityImageCollection
            {
                { "Image", updatedRecord }
            };
            
            PluginExecutionContextMock.Setup(x => x.PreEntityImages).Returns(imagesCollection);

            var outputParameters = new ParameterCollection
            {
                { "id", Guid.NewGuid() }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new PopulatePrimaryAttributePlugin();

            plugin.Execute(ServiceProvider);

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            if (!pluginExecutionContext.InputParameters.Contains("Target") || !(pluginExecutionContext.InputParameters["Target"] is Entity))
            {
                throw new Exception("Input target should be Entity.");
            }

            var exptected = $"{carrier.Name} - {cptCode.ipg_name}";
            
            var record = ((Entity)pluginExecutionContext.InputParameters["Target"]).ToEntity<ipg_associatedcpt>();

            Assert.Equal(exptected, record.ipg_name);
            #endregion
        }
    }
}