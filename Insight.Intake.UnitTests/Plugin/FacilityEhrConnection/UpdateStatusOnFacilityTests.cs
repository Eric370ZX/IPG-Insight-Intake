using FakeXrmEasy;
using Insight.Intake.Plugin.FacilityEhrConnection;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using ParameterCollection = Microsoft.Xrm.Sdk.ParameterCollection;

namespace Insight.Intake.UnitTests.Plugin.FacilityEhrConnection
{
    public class UpdateStatusOnFacilityTests : PluginTestsBase
    {
        [Fact]
        public void SetsActiveStatus()
        {
            //ARRANGE
            
            var facility = new Intake.Account().Fake()
                .WithEnrolledInEhr(Account_ipg_enrolledinehr.No)
                .Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithProcedureDates(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            //ACT

            var pluginContext = CreateContext(range1);
            fakedContext.ExecutePluginWith<UpdateStatusOnFacility>(pluginContext);

            //ASSERT
            
            var orgService = fakedContext.GetOrganizationService();
            var updatedFacility = orgService.Retrieve(Intake.Account.EntityLogicalName, facility.Id, new ColumnSet(Intake.Account.Fields.ipg_enrolledinehr)).ToEntity<Intake.Account>();
            Assert.AreEqual(Account_ipg_enrolledinehr.Yes, updatedFacility.ipg_enrolledinehrEnum);
        }

        [Fact]
        public void DoesNotSetActiveStatusIfNoActiveRange()
        {
            //ARRANGE

            var facility = new Intake.Account().Fake()
                .WithEnrolledInEhr(Account_ipg_enrolledinehr.No)
                .Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithProcedureDates(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            //ACT

            var pluginContext = CreateContext(range1);
            fakedContext.ExecutePluginWith<UpdateStatusOnFacility>(pluginContext);

            //ASSERT

            var orgService = fakedContext.GetOrganizationService();
            var updatedFacility = orgService.Retrieve(Intake.Account.EntityLogicalName, facility.Id, new ColumnSet(Intake.Account.Fields.ipg_enrolledinehr)).ToEntity<Intake.Account>();
            Assert.AreEqual(Account_ipg_enrolledinehr.No, updatedFacility.ipg_enrolledinehrEnum);
        }

        [Fact]
        public void DoesNotSetActiveStatusIfRangesInactiveOrIrrelevant()
        {
            //ARRANGE

            var facility1 = new Intake.Account().Fake()
                .WithEnrolledInEhr(Account_ipg_enrolledinehr.No)
                .Generate();
            var facility2 = new Intake.Account().Fake()
                .WithEnrolledInEhr(Account_ipg_enrolledinehr.No)
                .Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithStateCode(ipg_FacilityEhrConnectionState.Inactive)
                .WithFacilityReference(facility1)
                .WithProcedureDates(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10))
                .Generate();
            var range2 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility2)
                .WithProcedureDates(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility1, facility2, range1, range2
            });

            //ACT

            var pluginContext = CreateContext(range1);
            fakedContext.ExecutePluginWith<UpdateStatusOnFacility>(pluginContext);

            //ASSERT

            var orgService = fakedContext.GetOrganizationService();
            var updatedFacility = orgService.Retrieve(Intake.Account.EntityLogicalName, facility1.Id, new ColumnSet(Intake.Account.Fields.ipg_enrolledinehr)).ToEntity<Intake.Account>();
            Assert.AreEqual(Account_ipg_enrolledinehr.No, updatedFacility.ipg_enrolledinehrEnum);
        }


        private XrmFakedPluginExecutionContext CreateContext(ipg_FacilityEhrConnection target)
        {
            return new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_FacilityEhrConnection.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", target } },
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };
        }
    }
}
