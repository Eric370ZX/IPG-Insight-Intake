using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.CodeActivities
{
    public class GenerateFollowUpTaskOnHold : CodeActivity
    {
        [Input("Init task")]
        [ReferenceTarget("task")]
        [RequiredArgument]
        public InArgument<EntityReference> initTask { get; set; }
        [Input("Case")]
        [ReferenceTarget("incident")]
        [RequiredArgument]
        public InArgument<EntityReference> initCase { get; set; }
        [Input("Task type")]
        [RequiredArgument]
        [AttributeTarget("task", "ipg_tasktypecode")]
        public InArgument<OptionSetValue> initTaskType { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService crmService = serviceFactory.CreateOrganizationService(Guid.Empty);
            var traceService = context.GetExtension<ITracingService>();
            var initTaskRef = initTask.Get(context);
            var initCaseRef = initCase.Get(context);

            var taskType = (ipg_TaskType1?)initTaskType.Get(context)?.Value;
            var caseManager = new CaseManager(crmService, traceService, initCaseRef);
            caseManager.CreateFollowUpHoldTask(taskType);
        }
    }
}
