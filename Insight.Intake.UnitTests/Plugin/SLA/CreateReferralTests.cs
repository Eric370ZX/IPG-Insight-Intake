using FakeXrmEasy;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.Plugin.SLA;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.SLA
{
    public class CreateReferralTests
    {
        [Fact]
        public void CheckCreateReferral()
        {
            DateTime originalDocDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Create Referral")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Create_Referral)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(1);

            ipg_documenttype documentType = new ipg_documenttype()
                .Fake(Guid.NewGuid());

            ipg_document document = new ipg_document()
                .Fake(Guid.NewGuid())
                .WithDocumentTypeReference(documentType)
                .WithOriginalDocDate(originalDocDate)
                .WithOwnerReference(new EntityReference("systemsuser", new Guid()));

            ipg_referral referral = new ipg_referral()
                .Fake(Guid.NewGuid())
                .WithOrigin(Incident_CaseOriginCode.Fax)
                .WithCreatedOn(DateTime.UtcNow)
                .WithSourceDocument(document);

            var fakedEntities = new List<Entity>() { documentType, document, referral, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = referral.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateReferral>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet.FirstOrDefault(x => x.RegardingObjectId != null && x.RegardingObjectId.Id == referral.Id);

                Assert.Equal(task.ipg_tasktypeid?.Id, taskType.Id);
                Assert.Equal(referral.CreatedOn, task.ActualEnd);
                Assert.Equal(TaskState.Completed, task.StateCode);
            }
        }
    }
}
