using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class SetRelatedToInsuredTests : PluginTestsBase
    {
        [Fact]
        public void SetRelatedToInsured_Success()
        {
            //ARRANGE
            var patient = new Intake.Contact()
            {
                FirstName = "First name",
                LastName = "Last name"
            }
                .Fake()
                .Generate();

            Incident incident = new Incident().Fake()
                    .WithPatientReference(patient);




            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, patient});

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
            fakedContext.ExecutePluginWith<SetRelatedToInsured>(pluginContext);

            //ASSERT
            Assert.Equal(patient.FirstName, incident.ipg_InsuredFirstName);
        }


    }
}
