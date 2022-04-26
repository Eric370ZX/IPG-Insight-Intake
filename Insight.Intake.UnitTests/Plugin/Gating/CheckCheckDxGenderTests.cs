using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckCheckDxGenderTests : PluginTestsBase
    {
        [Fact]
        public void CheckCheckDxGenderTests_AppropriateGenderAll_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            Insight.Intake.ipg_dxcode dxCode = new Intake.ipg_dxcode().Fake();
            dxCode.ipg_genderEnum = ipg_Gender.Other;

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p=>p.ipg_DxCodeId1,p=>dxCode.ToEntityReference());
            caseEntity.ipg_PatientGender = new OptionSetValue((int)ipg_Gender.Male);
            var listForInit = new List<Entity>() { caseEntity, dxCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckDxGender",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckDxGender>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckCheckDxGenderTests_AppropriateGenderMale_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            Insight.Intake.ipg_dxcode dxCode = new Intake.ipg_dxcode().Fake();
            dxCode.ipg_genderEnum = ipg_Gender.Male;

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_DxCodeId2, p => dxCode.ToEntityReference());
            caseEntity.ipg_PatientGender = new OptionSetValue((int)ipg_Gender.Male);
            var listForInit = new List<Entity>() { caseEntity, dxCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckDxGender",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckDxGender>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckCheckDxGenderTests_MismatchToPatientGender_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            Insight.Intake.ipg_dxcode dxCode = new Intake.ipg_dxcode().Fake();
            dxCode.ipg_genderEnum = ipg_Gender.Male;
            Insight.Intake.Contact patient = new Intake.Contact().Fake();
            patient.GenderCodeEnum = Contact_GenderCode.Female;
            Incident caseEntity = new Incident().Fake()
                .WithPatientReference(patient)
                .RuleFor(p => p.ipg_DxCodeId2, p => dxCode.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity, dxCode, patient };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckDxGender",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckDxGender>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
