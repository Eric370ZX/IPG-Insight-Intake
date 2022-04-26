using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class CreateAnnotationFromReferralNoteTests : PluginTestsBase
    {
        [Fact]
        public void CreateAnnotation()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var referral = new ipg_referral() {Id = Guid.NewGuid(), ipg_referralnote = "test" };
            var listForInit = new List<Entity>() { referral };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", referral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            //ACT
            fakedContext.ExecutePluginWith<CreateAnnotationFromReferralNote>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var createdAnnotation = crmContext.CreateQuery<Annotation>().FirstOrDefault();

            Assert.NotNull(createdAnnotation);
            Assert.Equal(referral.ipg_referralnote, createdAnnotation.NoteText);
        }

    }
}
