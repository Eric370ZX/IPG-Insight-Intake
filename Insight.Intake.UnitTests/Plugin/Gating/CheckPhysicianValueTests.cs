using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckPhysicianValueTests : PluginTestsBase
    {
        [Fact]
        public void CheckPhysicianValueTests_CasePhysicianIsValid_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Contact physician = new Contact().Fake();
            physician.StateCode = ContactState.Active;
            physician.StatusCode = new OptionSetValue((int)Contact_StatusCode.Active);
            physician.ipg_approved = true;
            Intake.Account facility = new Intake.Account().Fake();
            Incident caseEntity = new Incident().Fake().RuleFor(p => p.ipg_FacilityId, p => facility.ToEntityReference());
            caseEntity.ipg_PhysicianId = physician.ToEntityReference();
            ipg_facilityphysician facilityPhysician = new ipg_facilityphysician()
                .Fake()
                .RuleFor(p => p.ipg_status, true)
                .RuleFor(p => p.ipg_facilityid, p => facility.ToEntityReference())
                .RuleFor(p => p.ipg_physicianid, p => physician.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity, physician, facility, facilityPhysician };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPhysicianValue",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPhysicianValue>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckPhysicianValueTests_ReferralPhysicianIsValid_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Contact physician = new Contact().Fake();
            physician.StateCode = ContactState.Active;
            physician.StatusCode = new OptionSetValue((int)Contact_StatusCode.Approved);
            Intake.Account facility = new Intake.Account().Fake();
            ipg_referral refEntity = new ipg_referral().Fake().RuleFor(p => p.ipg_FacilityId, p => facility.ToEntityReference());
            refEntity.ipg_PhysicianId = physician.ToEntityReference();

            var listForInit = new List<Entity>() { refEntity, physician, facility };
            fakedContext.Initialize(listForInit);

            fakedContext.AddRelationship(ipg_physician_facility.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_physician_facility.EntityLogicalName,
                Entity1LogicalName = Contact.EntityLogicalName,
                Entity1Attribute = Contact.PrimaryIdAttribute,
                Entity2LogicalName = Intake.Account.EntityLogicalName,
                Entity2Attribute = Intake.Account.PrimaryIdAttribute
            });

            var request = new AssociateRequest()
            {
                Target = physician.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    facility.ToEntityReference(),
                },
                Relationship = new Relationship(ipg_physician_facility.EntityLogicalName)
            };
            fakedService.Execute(request);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPhysicianValue",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPhysicianValue>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckPhysicianValueTests_NoRelation_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Contact physician = new Contact().Fake();
            physician.StateCode = ContactState.Active;
            physician.StatusCode = new OptionSetValue((int)Contact_StatusCode.Approved);
            Intake.Account facility = new Intake.Account().Fake();
            Incident caseEntity = new Incident().Fake().RuleFor(p => p.ipg_FacilityId, p => facility.ToEntityReference());
            caseEntity.ipg_PhysicianId = physician.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, physician, facility };
            fakedContext.Initialize(listForInit);

            fakedContext.AddRelationship(ipg_physician_facility.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_physician_facility.EntityLogicalName,
                Entity1LogicalName = Contact.EntityLogicalName,
                Entity1Attribute = Contact.PrimaryIdAttribute,
                Entity2LogicalName = Intake.Account.EntityLogicalName,
                Entity2Attribute = Intake.Account.PrimaryIdAttribute
            });

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPhysicianValue",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPhysicianValue>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}