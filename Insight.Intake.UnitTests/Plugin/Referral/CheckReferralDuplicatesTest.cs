using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
	public class CheckReferralDuplicatesTest : PluginTestsBase
	{
		[Fact]
		public void CheckReferralDuplicates_SomeFieldsAreNull_returnError()
		{
			//arrange
			var fakedContext = new XrmFakedContext();

			ipg_referral refEntity1 = new ipg_referral().Fake();
			refEntity1.Id = Guid.NewGuid();
			refEntity1.ipg_PatientFirstName = "A";
			refEntity1.ipg_PatientLastName = "B";
			refEntity1.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			refEntity1.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);


			ipg_referral refEntity2 = new ipg_referral().Fake();
			refEntity2.Id = Guid.NewGuid();
			refEntity2.ipg_PatientFirstName = null;
			refEntity2.ipg_PatientLastName = null;
			refEntity2.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			refEntity2.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);

			var listForInit = new List<Entity>() { refEntity1, refEntity2 };
			fakedContext.Initialize(listForInit);

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = MessageNames.Create,
				Stage = PipelineStages.PostOperation,
				PrimaryEntityName = ipg_referral.EntityLogicalName,
				InputParameters = new ParameterCollection() { { "Target", refEntity2 } },
				OutputParameters = new ParameterCollection(),
				PostEntityImages = new EntityImageCollection() { { "PostImage", refEntity2 } },
				PreEntityImages = new EntityImageCollection(),
				InitiatingUserId = Guid.NewGuid()
			};
			//ACT
			fakedContext.ExecutePluginWith<CheckReferralDuplicate>(pluginContext);

			//Assert
			var fakedService = fakedContext.GetOrganizationService();
			var crmContext = new OrganizationServiceContext(fakedService);

			var createdReferrals = crmContext.CreateQuery<ipg_referral>().Where(referral => 
				referral.Id == refEntity2.Id
				&& referral.ipg_casestatusEnum == null
				&& referral.ipg_ReasonsEnum == null).ToList();
			Assert.Single(createdReferrals);
		}
		[Fact]
		public void CheckReferralDuplicates_DetectDuplication_returnError()
		{
			//arrange
			var fakedContext = new XrmFakedContext();

			ipg_referral refEntity1 = new ipg_referral().Fake();
			refEntity1.Id = Guid.NewGuid();
			refEntity1.ipg_PatientFirstName = "A";
			refEntity1.ipg_PatientLastName = "B";
			refEntity1.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			refEntity1.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);


			ipg_referral refEntity2 = new ipg_referral().Fake();
			refEntity2.Id = Guid.NewGuid();
			refEntity2.ipg_PatientFirstName = "A";
			refEntity2.ipg_PatientLastName = "B";
			refEntity2.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			refEntity2.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);

			var listForInit = new List<Entity>() { refEntity1, refEntity2 };
			fakedContext.Initialize(listForInit);

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = MessageNames.Create,
				Stage = PipelineStages.PostOperation,
				PrimaryEntityName = ipg_referral.EntityLogicalName,
				InputParameters = new ParameterCollection() { { "Target", refEntity2 } },
				OutputParameters = new ParameterCollection(),
				PostEntityImages = new EntityImageCollection() { { "PostImage", refEntity2 } },
				PreEntityImages = new EntityImageCollection(),
				InitiatingUserId = Guid.NewGuid()
			};
			//ACT
			fakedContext.ExecutePluginWith<CheckReferralDuplicate>(pluginContext);

			//Assert
			var fakedService = fakedContext.GetOrganizationService();
			var crmContext = new OrganizationServiceContext(fakedService);

			var createdReferrals = crmContext.CreateQuery<ipg_referral>().Where(referral =>
				referral.Id == refEntity2.Id
				&& referral.ipg_casestatusEnum == ipg_CaseStatus.Closed
				&& referral.ipg_ReasonsEnum == ipg_CaseReasons.DuplicateReferralORDuplicateCase).ToList();
			Assert.Single(createdReferrals);
		}

		[Fact]
		public void CheckReferralDuplicates_DetectNotActiveDuplication_Succeed()
		{
			//arrange
			var fakedContext = new XrmFakedContext();

			ipg_referral refEntity1 = new ipg_referral().Fake();
			refEntity1.Id = Guid.NewGuid();
			refEntity1.ipg_PatientFirstName = "A";
			refEntity1.ipg_PatientLastName = "B";
			refEntity1.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			refEntity1.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);
			refEntity1.StateCode = ipg_referralState.Inactive;


			ipg_referral refEntity2 = new ipg_referral().Fake();
			refEntity2.Id = Guid.NewGuid();
			refEntity2.ipg_PatientFirstName = "A";
			refEntity2.ipg_PatientLastName = "B";
			refEntity2.ipg_PatientDateofBirth = DateTime.Now.AddYears(-35);
			refEntity2.ipg_SurgeryDate = DateTime.Now.AddMonths(-1);

			var listForInit = new List<Entity>() { refEntity1, refEntity2 };
			fakedContext.Initialize(listForInit);

			var pluginContext = new XrmFakedPluginExecutionContext()
			{
				MessageName = MessageNames.Create,
				Stage = PipelineStages.PostOperation,
				PrimaryEntityName = ipg_referral.EntityLogicalName,
				InputParameters = new ParameterCollection() { { "Target", refEntity2 } },
				OutputParameters = new ParameterCollection(),
				PostEntityImages = new EntityImageCollection() { { "PostImage", refEntity2 } },
				PreEntityImages = new EntityImageCollection(),
				InitiatingUserId = Guid.NewGuid()
			};
			//ACT
			fakedContext.ExecutePluginWith<CheckReferralDuplicate>(pluginContext);

			//Assert
			var fakedService = fakedContext.GetOrganizationService();
			var crmContext = new OrganizationServiceContext(fakedService);

			var createdReferrals = crmContext.CreateQuery<ipg_referral>().Where(referral =>
				referral.Id == refEntity2.Id
				&& referral.ipg_casestatusEnum != ipg_CaseStatus.Closed
				&& referral.ipg_ReasonsEnum != ipg_CaseReasons.DuplicateReferralORDuplicateCase).ToList();
			Assert.Single(createdReferrals);
		}
	}
}
