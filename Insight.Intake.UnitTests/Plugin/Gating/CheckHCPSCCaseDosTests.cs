using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckHCPSCCaseDosTests : PluginTestsBase
    {
        [Fact]
        public void CheckHCPSCCaseDosTests_NoCaseDos_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckHCPSCCaseDos",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckHCPSCCaseDos>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckHCPSCCaseDosTests_CaseDosInRange_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake().WithActualDos(new DateTime(2020, 01, 25));

            ipg_masterhcpcs hcpcs1 = new ipg_masterhcpcs().Fake();
            hcpcs1.ipg_EffectiveDate = new DateTime(2020, 01, 01);
            hcpcs1.ipg_ExpirationDate = new DateTime(2020, 02, 01);

            ipg_masterhcpcs hcpcs2 = new ipg_masterhcpcs().Fake();
            hcpcs2.ipg_EffectiveDate = new DateTime(2020, 01, 10);
            hcpcs2.ipg_ExpirationDate = new DateTime(2020, 02, 15);

            ipg_casepartdetail part1 = new ipg_casepartdetail().Fake().WithCaseReference(caseEntity);
            part1.ipg_hcpcscode = hcpcs1.ToEntityReference();

            ipg_casepartdetail part2 = new ipg_casepartdetail().Fake().WithCaseReference(caseEntity);
            part2.ipg_hcpcscode = hcpcs2.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, hcpcs1, hcpcs2, part1, part2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckHCPSCCaseDos",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckHCPSCCaseDos>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckHCPSCCaseDosTests_CaseDosOutOfRange_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake().WithActualDos(new DateTime(2020, 03, 25));

            ipg_masterhcpcs hcpcs1 = new ipg_masterhcpcs().Fake();
            hcpcs1.ipg_EffectiveDate = new DateTime(2020, 01, 01);
            hcpcs1.ipg_ExpirationDate = new DateTime(2020, 02, 01);

            ipg_masterhcpcs hcpcs2 = new ipg_masterhcpcs().Fake();
            hcpcs2.ipg_EffectiveDate = new DateTime(2020, 01, 10);
            hcpcs2.ipg_ExpirationDate = new DateTime(2020, 02, 15);

            ipg_casepartdetail part1 = new ipg_casepartdetail().Fake().WithCaseReference(caseEntity);
            part1.ipg_hcpcscode = hcpcs1.ToEntityReference();

            ipg_casepartdetail part2 = new ipg_casepartdetail().Fake().WithCaseReference(caseEntity);
            part2.ipg_hcpcscode = hcpcs2.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, hcpcs1, hcpcs2, part1, part2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckHCPSCCaseDos",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckHCPSCCaseDos>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
