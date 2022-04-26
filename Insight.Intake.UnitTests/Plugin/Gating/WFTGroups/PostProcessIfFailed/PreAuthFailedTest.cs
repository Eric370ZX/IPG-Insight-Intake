using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.Plugin.Gating.WFTGroups.PostProcessIfFailed;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class PreAuthFailedTestTest : PluginTestsBase
    {
        [Fact]
        public void PreAuthFailedTest_returnSuccess()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            var listForInit = new List<Entity>() { refCase };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingGroupPreAuthFailed",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<PreAuthFailed>(pluginContext);
            var resultCase = fakedContext
                .GetOrganizationService()
                .Retrieve(refCase.LogicalName, refCase.Id, new ColumnSet(true))
                .ToEntity<Incident>();
            //Assert
            Assert.True(resultCase.ipg_is_authorization_required);
        }
    }
}
