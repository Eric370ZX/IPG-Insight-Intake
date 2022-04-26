using FakeXrmEasy;
using Insight.Intake.Plugin.FacilityEhrConnection;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using ParameterCollection = Microsoft.Xrm.Sdk.ParameterCollection;

namespace Insight.Intake.UnitTests.Plugin.FacilityEhrConnection
{
    public class EhrConnectionDatesValidationPluginTest : PluginTestsBase
    {
        [Fact]
        public void ThrowsExceptionIfInvalidProcedureDateRange()
        {
            var facility = new Intake.Account().Fake().Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 6, 1), new DateTime(2020, 5, 1))
                .Generate();

            var pluginContext = CreateContext(newRange);

            var exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext));
        }

        [Fact]
        public void ThrowsExceptionIfRangesWithBothDatesOverlap()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), new DateTime(2020, 6, 30))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 6, 1), new DateTime(2020, 12, 31))
                .Generate();

            var pluginContext = CreateContext(newRange);

            var exception =  Assert.ThrowsException<InvalidPluginExecutionException>(()=>fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext));
        }

        [Fact]
        public void NoExceptionIfRangesWithBothDatesDoNotOverlap()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), new DateTime(2020, 6, 30))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 7, 1), new DateTime(2020, 12, 31))
                .Generate();

            var pluginContext = CreateContext(newRange);

            fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext);
        }

        [Fact]
        public void ThrowsExceptionIfExistingRangeWithoutExpirationDateOverlaps()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), null)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 12, 1), new DateTime(2020, 12, 31))
                .Generate();

            var pluginContext = CreateContext(newRange);

            var exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext));
        }

        [Fact]
        public void ThrowsExceptionIfNewRangeWithoutExpirationDateOverlaps()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 2, 1), new DateTime(2020, 3, 1))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), null)
                .Generate();

            var pluginContext = CreateContext(newRange);

            var exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext));
        }

        [Fact]
        public void NoExceptionIfExistingRangeWithoutExpirationDateDoesNotOverlap()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 2, 1), null)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), new DateTime(2020, 1, 20))
                .Generate();

            var pluginContext = CreateContext(newRange);

            fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext);
        }

        [Fact]
        public void NoExceptionIfNewRangeWithoutExpirationDateDoesNotOverlap()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 2, 1), new DateTime(2020, 3, 1))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 4, 1), null)
                .Generate();

            var pluginContext = CreateContext(newRange);

            fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext);
        }

        [Fact]
        public void ThrowsExceptionIfNewRangeIsInsideExisting()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), new DateTime(2020, 12, 31))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 6, 15), new DateTime(2020, 7, 15))
                .Generate();

            var pluginContext = CreateContext(newRange);

            var exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext));
        }

        [Fact]
        public void ThrowsExceptionIfExistingRangeIsInsideNew()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 5, 1), new DateTime(2020, 6, 1))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), new DateTime(2020, 12, 31))
                .Generate();

            var pluginContext = CreateContext(newRange);

            var exception = Assert.ThrowsException<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext));
        }

        [Fact]
        public void NoExceptionIfRangesOverlapButDifferentSoftware()
        {
            var facility = new Intake.Account().Fake().Generate();

            var range1 = new Intake.ipg_FacilityEhrConnection().Fake()
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.HST)
                .WithProcedureDates(new DateTime(2020, 1, 1), new DateTime(2020, 6, 30))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>
            {
                facility, range1
            });

            var newRange = new Intake.ipg_FacilityEhrConnection().Fake()
                .WithId(null)
                .Active()
                .WithFacilityReference(facility)
                .WithSoftware(ipg_EHRsoftware.AdvantX)
                .WithProcedureDates(new DateTime(2020, 6, 1), new DateTime(2020, 12, 31))
                .Generate();

            var pluginContext = CreateContext(newRange);

            fakedContext.ExecutePluginWith<EhrConnectionDatesValidationPlugin>(pluginContext);
        }

        private XrmFakedPluginExecutionContext CreateContext(ipg_FacilityEhrConnection target)
        {
            return new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = ipg_FacilityEhrConnection.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", target } },
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };
        }
    }
}
