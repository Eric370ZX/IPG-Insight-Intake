using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class CreateCasePluginTests: PluginTestsBase
    {
		[Fact]
		public void CheckInformationCopiedOnCaseCreateAction()
		{
			//arrange
			var fakedContext = new XrmFakedContext();
			Intake.Account carrier = new Intake.Account().FakeCarrierForEBV();
			ipg_referral referral = new ipg_referral().Fake().WithCarrierReference(carrier);
			referral.ipg_PatientFirstName = "A";
			referral.ipg_PatientLastName = "B";
			referral.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			referral.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);
			referral.ipg_EHRDataSource = "EhrDataSource1";
			referral.ipg_EHRFileFormatVersion = "2.3.1";
			referral.ipg_EHRCaseId = "12345";

			var listForInit = new List<Entity>() { referral, carrier };
			fakedContext.Initialize(listForInit);

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = "ipg_IPGReferralCreateCase",
				Stage = PipelineStages.PostOperation,
				PrimaryEntityName = ipg_referral.EntityLogicalName,
				InputParameters = new ParameterCollection() { { "Target", referral.ToEntityReference() } },
				OutputParameters = new ParameterCollection(),
				PostEntityImages = new EntityImageCollection() {},
				PreEntityImages = new EntityImageCollection(),
				InitiatingUserId = Guid.NewGuid()
			};
			//ACT
			fakedContext.ExecutePluginWith<CreateCasePlugin>(pluginContext);

			//Assert
			var fakedService = fakedContext.GetOrganizationService();
			var crmContext = new OrganizationServiceContext(fakedService);

			var createdCase = crmContext.CreateQuery<Incident>().Where(incident =>
				incident.ipg_ReferralId.Id == referral.Id).FirstOrDefault();
			var contextreferral = crmContext.CreateQuery<ipg_referral>().Where(referr =>
			referr.Id == referral.Id).FirstOrDefault();

			Assert.True(pluginContext.OutputParameters.Contains("Case"));
			Assert.NotNull(pluginContext.OutputParameters["Case"]);
			Assert.NotNull(createdCase);

			Assert.Equal(referral.ipg_EHRDataSource, createdCase.ipg_EHRDataSource);
			Assert.Equal(referral.ipg_EHRFileFormatVersion, createdCase.ipg_EhrFileFormatVersion);
			Assert.Equal(referral.ipg_EHRCaseId, createdCase.ipg_EHRCaseId);

			Assert.Equal(contextreferral.ipg_AssociatedCaseId, createdCase.ToEntityReference());


		}

		[Fact(Skip = "Relevant for integation tests only")]
		public void CreateCaseIntegrationTest()
		{
			//arrange
			var realContext = new XrmRealContext()
			{ 
			
			};
			//Intake.Account carrier = new Intake.Account().FakeCarrierForEBV();
			//ipg_referral referral = new ipg_referral().Fake().WithCarrierReference(carrier);
			//referral.ipg_PatientFirstName = "A";
			//referral.ipg_PatientLastName = "B";
			//referral.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			//referral.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);



			//var listForInit = new List<Entity>() { referral, carrier };
			//realContext.Initialize(listForInit);

			var referral = new EntityReference(ipg_referral.EntityLogicalName, new Guid("635cda8b-a838-eb11-a813-000d3a59862f"));

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = "ipg_IPGReferralCreateCase",
				Stage = PipelineStages.PostOperation,
				PrimaryEntityName = ipg_referral.EntityLogicalName,
				InputParameters = new ParameterCollection() { { "Target", referral } },
				OutputParameters = new ParameterCollection(),
				PostEntityImages = new EntityImageCollection() { },
				PreEntityImages = new EntityImageCollection(),
				InitiatingUserId = Guid.NewGuid()
			};
			//ACT
			realContext.ExecutePluginWith<CreateCasePlugin>(pluginContext);

			Assert.True(true);
		}
	}
}
