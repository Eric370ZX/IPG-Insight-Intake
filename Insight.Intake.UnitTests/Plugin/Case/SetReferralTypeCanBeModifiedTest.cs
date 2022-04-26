using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class SetReferralTypeCanBeModifiedTest : PluginTestsBase
    {
        [Fact]
        public void ValidateReferralTypeCanBeModified()
        { 
        
            #region Arrange
            var fakedContext = new XrmFakedContext();

            ServiceProvider = ServiceProviderMock.Object;

            Incident createdIncident = new Incident().Fake().WithScheduledDos(DateTime.Now.AddDays(5)).WithReferralTypeHasBeenModified(false);
            
            OrganizationServiceMock.WithUpdateCrud<Incident>();

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Act

            fakedContext.Initialize(createdIncident);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdIncident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", createdIncident } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };


            fakedContext.ExecutePluginWith<SetReferralTypeCanBeModified>(pluginContext);

            #endregion

            #region Assert
            Assert.Equal(true, createdIncident.ipg_referraltypecanbeaccessed);
            Assert.Equal(false, createdIncident.ipg_referraltypehasbeenmodified);
            #endregion
        }


        [Fact]
        public void ValidateReferralTypeHasBeenModified()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            ServiceProvider = ServiceProviderMock.Object;

            Incident createdIncident = new Incident().Fake().WithReferralTypeHasBeenModified(false).WithDosHasBeenModified(true).WithReferralType(427880002);

            OrganizationServiceMock.WithUpdateCrud<Incident>();

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup execution context

            fakedContext.Initialize(createdIncident);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdIncident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", createdIncident } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };


            fakedContext.ExecutePluginWith<SetReferralTypeCanBeModified>(pluginContext);

            #endregion

            #region Assert
            Assert.Equal(false, createdIncident.ipg_referraltypecanbeaccessed);
            Assert.Equal(true, createdIncident.ipg_referraltypehasbeenmodified);
            #endregion
        }
    }
}
