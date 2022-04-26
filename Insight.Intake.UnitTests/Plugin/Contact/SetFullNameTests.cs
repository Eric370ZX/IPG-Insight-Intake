using FakeXrmEasy;
using Insight.Intake.Plugin.Contact;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class SetFullNameTests : PluginTestsBase
    {
        [Fact]
        public void CheckFullName()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();

            Contact fakedContact = new Contact();
            fakedContact.FirstName = "Fn";
            fakedContact.MiddleName = "Mn";
            fakedContact.LastName = "Ln";

            var inputParameters = new ParameterCollection() { { "Target", fakedContact } };

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = fakedContact.LogicalName,
                InputParameters = inputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<SetFullName>(fakedPluginExecutionContext);

            var updatedContact = fakedContact;

            // Assert
            Assert.Equal("Ln, Fn Mn", updatedContact.ipg_fullname);
        }


    }
}
