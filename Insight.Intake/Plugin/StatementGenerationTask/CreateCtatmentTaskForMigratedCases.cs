using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.StatementGenerationTask
{
    public class CreateCtatmentTaskForMigratedCases : PluginBase
    {
        private PatientStatementTaskRepository repository;
        public CreateCtatmentTaskForMigratedCases() : base(typeof(CreateCtatmentTaskForMigratedCases))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, CreateStatementTasksForMigratedCases);
        }

        private void CreateStatementTasksForMigratedCases(LocalPluginContext localPluginContext)
        {
            if (ShouldGenerateStatementTask(localPluginContext))
            {
                CreateStatementGenerationTask(localPluginContext);
            }
        }

        private void CreateStatementGenerationTask(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Incident>();
            var repository = new PatientStatementTaskRepository(localPluginContext.OrganizationService, localPluginContext.TracingService);
            var statementType = repository.FindStatementGenerationConfig(target.ipg_LastStatementType.Name);
            repository.CreateTask(target.ToEntityReference(), statementType, DateTime.Now);
        }
        private bool IsImportUser(LocalPluginContext localContext)
        {
            var context = new OrganizationServiceContext(localContext.OrganizationService);
            var incident = localContext.Target<Incident>();
            var importUser = (from user in context.CreateQuery<SystemUser>()
                      where user.FullName == UserNames.ImportUserName
                      select user).FirstOrDefault();

            return importUser?.Id == incident?.CreatedBy?.Id;
        }

        public bool ShouldGenerateStatementTask(LocalPluginContext context)
        {
            var incident = context.Target<Incident>();

            return incident.ipg_CaseStatusEnum.HasValue && incident.ipg_StateCodeEnum.HasValue &&
            IsImportUser(context) &&
            incident.ipg_CaseStatusEnum.Value == ipg_CaseStatus.Open &&
            (incident.ipg_StateCodeEnum.Value == ipg_CaseStateCodes.CarrierServices || incident.ipg_StateCodeEnum.Value == ipg_CaseStateCodes.PatientServices);
        }
    }
}
