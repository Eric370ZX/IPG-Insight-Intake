using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class SetClosedDateTests : PluginTestsBase
    {

        [Fact]
        public void SetClosedDateTest_CaseStatusChangedToClosed_SetClosedDate() 
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referralPreImage = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Open);
            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Closed);
            var listForInit = new List<Entity> { referralPreImage, referral };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = referral.LogicalName,
                InputParameters = new ParameterCollection() { { "Target", referral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection() { { "PreImage", referralPreImage } }
            };

            //Act

            fakedContext.ExecutePluginWith<SetClosedDate>(pluginContext);
            var updatedReferral = referral;

            ///Assert
            Assert.NotNull(updatedReferral.ipg_closeddate);
        }
        [Fact]
        public void SetClosedDateTest_CaseStatusNotChanged_ClosedDateNotChanged()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referralPreImage = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Closed);
            referralPreImage.ipg_closeddate = DateTime.Now;
            ipg_referral referral = new ipg_referral().Fake().WithCaseStatus(ipg_CaseStatus.Closed);
            referral.ipg_closeddate = referralPreImage.ipg_closeddate;
            var listForInit = new List<Entity> { referralPreImage, referral };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = referral.LogicalName,
                InputParameters = new ParameterCollection() { { "Target", referral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection() { { "PreImage", referralPreImage } }
            };

            //Act

            fakedContext.ExecutePluginWith<SetClosedDate>(pluginContext);
            var updatedReferral = referral;

            ///Assert
            Assert.Equal(referralPreImage.ipg_closeddate, updatedReferral.ipg_closeddate);
        }
    }   
}

