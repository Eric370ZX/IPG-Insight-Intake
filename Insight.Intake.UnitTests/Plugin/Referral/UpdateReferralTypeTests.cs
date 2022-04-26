using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class UpdateReferralTypeTests: PluginTestsBase
    {
        [Fact]
        public void SetsRetroReferralTypeOnCreation()
        {
            //ARRANGE
            var referral = new ipg_referral().Fake()
                .WithSurgeryDate(DateTime.Now.AddDays(-1))
                .Generate();
            
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { referral });

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateReferralType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_ReferralType.Retro, referral.ipg_referraltypeEnum);
        }

        [Fact]
        public void SetsStandardReferralTypeOnCreation()
        {
            //ARRANGE
            var referral = new ipg_referral().Fake()
                .WithSurgeryDate(DateTime.Now.AddDays(5))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { referral });

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateReferralType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_ReferralType.Standard, referral.ipg_referraltypeEnum);
        }

        [Fact]
        public void SetsStatReferralTypeOnCreation()
        {
            //ARRANGE
            DateTime businessDay = GetBusinessDay();

            var referral = new ipg_referral().Fake()
                .WithSurgeryDate(businessDay)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { referral });

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateReferralType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_ReferralType.Stat, referral.ipg_referraltypeEnum);
        }

        [Fact]
        public void SetsUrgentReferralTypeOnCreation()
        {
            //ARRANGE
            DateTime businessDay = GetBusinessDay();

            var referral = new ipg_referral().Fake()
                .WithSurgeryDate(businessDay.AddDays(2))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { referral });

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateReferralType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_ReferralType.Urgent, referral.ipg_referraltypeEnum);
        }

        [Fact]
        public void ChangesReferralTypeOnUpdate()
        {
            //ARRANGE
            var createdOn = DateTime.Now.AddDays(-1);
            var referralPreImage = new ipg_referral().Fake()
                .WithReferralType(ipg_ReferralType.Standard)
                .WithCreatedOn(createdOn)
                .Generate();

            var referral = new ipg_referral().Fake(referralPreImage.Id)
                .WithSurgeryDate(createdOn.AddDays(-1))
                .WithCreatedOn(createdOn)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { referral });

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection { { "PreImage", referralPreImage} }
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateReferralType>(pluginContext);

            //ASSERT
            var orgService = fakedContext.GetOrganizationService();
            var updatedReferral = orgService.Retrieve(ipg_referral.EntityLogicalName, referral.Id, new ColumnSet(true)).ToEntity<ipg_referral>();
            Assert.Equal(ipg_ReferralType.Retro, updatedReferral.ipg_referraltypeEnum);
        }

        private DateTime GetBusinessDay()
        {
            DateTime businessDay = DateTime.Now.ToLocalTime().Date;
            if (businessDay.DayOfWeek == DayOfWeek.Saturday)
            {
                businessDay = businessDay.AddDays(2);
            }
            else if(businessDay.DayOfWeek == DayOfWeek.Sunday)
            {
                businessDay = businessDay.AddDays(1);
            }

            return businessDay;
        }

    }
}
