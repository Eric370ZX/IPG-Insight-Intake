using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class AddNewBenefitNoteAndAuthorizationNoteTests : PluginTestsBase
    {
        [Fact]
        public void AddNewBenefitNoteAndAuthorizationNote_NewBenefitNoteIsAdded_returnBenefitNoteIsNewBenefitNote()
        {
            var fakedContext = new XrmFakedContext();

            var textNote = "Test";
            Incident incident = new Incident().Fake();
            incident.ipg_newbenefitsnote = textNote;

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<AddNewBenefitNoteAndAuthorizationNote>(pluginContext);

            Assert.True(incident.ipg_benefitsnotes == textNote);
            Assert.True(incident.ipg_newbenefitsnote == string.Empty);
        }

        [Fact]
        public void AddNewBenefitNoteAndAuthorizationNote_NewAuthorizatioNoteIsAdded_returnAuthorizatioNoteIsNewAuthorizatioNote()
        {
            var fakedContext = new XrmFakedContext();

            var textNote = "Test";
            Incident incident = new Incident().Fake();
            incident.ipg_newauthorizationnote = textNote;

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<AddNewBenefitNoteAndAuthorizationNote>(pluginContext);

            Assert.True(incident.ipg_authorizationnotes == textNote);
            Assert.True(incident.ipg_newauthorizationnote == string.Empty);
        }
    }
    
}
