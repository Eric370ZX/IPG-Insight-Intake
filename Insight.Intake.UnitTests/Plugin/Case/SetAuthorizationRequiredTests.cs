using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class SetAuthorizationRequiredTests : PluginTestsBase
    {
        [Fact]
        public void HasAutoCarrier_Success()
        {
            //ARRANGE
            var firstCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Auto)
                    .Generate();
            var secondCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Commercial)
                    .Generate();
            Incident incident = new Incident().Fake()
                    .WithCarrierReference(firstCarrier);
                   
                    

            
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, firstCarrier, secondCarrier });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<SetAuthorizationRequired>(pluginContext);

            //ASSERT
            Assert.Equal(false, incident.ipg_is_authorization_required);
        }

        [Fact]
        public void HasNoCarrier_Success()
        {
            //ARRANGE
            var firstCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Government)
                    .Generate();
            var secondCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Commercial)
                    .Generate();
            Incident incident = new Incident().Fake()
                    .WithCarrierReference(firstCarrier)
                    .RuleFor(p => p.ipg_SecondaryCarrierId, p => secondCarrier.ToEntityReference());




            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, firstCarrier, secondCarrier });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<SetAuthorizationRequired>(pluginContext);

            //ASSERT
            Assert.Null(incident.ipg_is_authorization_required);
        }
    }
}
