using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.CodeActivities
{
    public class CancelPrimaryEntityWorkflows : CodeActivity
    {
        private readonly string[] _notRethrowExceptionMessages = {
            "The state transition requested is not valid for the current state. Current state: 3, current status: 32, target state: 3",
            "The state transition requested is not valid for the current state. Current state: 2, current status: 22, target state: 3",
            "22 is not a valid status code for state code AsyncOperationState.Completed on asyncoperation",
            "The state transition requested is not valid for the current state. Current state: 3, current status: 30, target state: 3",
            "22 is not a valid status code for state code AsyncOperationState.Suspended on asyncoperation with Id",
            "Does Not Exist"
        };

        protected override void Execute(CodeActivityContext executionContext)
        {
            var wfName = WorkflowName.Get(executionContext);
            if (!String.IsNullOrEmpty(wfName))
            {
                var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
                var crm = serviceFactory.CreateOrganizationService(Guid.Empty);

                var ctx = executionContext.GetExtension<IWorkflowContext>();

                CancelWorkflows(ctx, crm, wfName, this.CancelThisWorkflow.Get(executionContext));
            }

        }

        private void CancelWorkflows(IWorkflowContext context, IOrganizationService crm, string workflowName, bool cancelThisWorkflow)
        {
            //create filter
            var filter = new FilterExpression { FilterOperator = LogicalOperator.And };
            filter.AddCondition("name", ConditionOperator.Like, workflowName);
            filter.AddCondition("regardingobjectid", ConditionOperator.Equal, context.PrimaryEntityId);
            filter.AddCondition("statuscode", ConditionOperator.In,
                (int)AsyncOperationStatus.InProgress,
                            (int)AsyncOperationStatus.Pausing,
                            (int)AsyncOperationStatus.Waiting,
                            (int)AsyncOperationStatus.WaitingForResources);
            if (!cancelThisWorkflow)
            {
                filter.AddCondition("asyncoperationid", ConditionOperator.NotEqual, context.OperationId);
            }

            //create query
            QueryExpression query = new QueryExpression("asyncoperation")
            {
                ColumnSet = new ColumnSet("asyncoperationid"),
                Criteria = filter
            };


            //execute query: retrieve running workflows
            var becoll = crm.RetrieveMultiple(query);
            var entities = becoll.Entities;

            //cancel workflows
            if (entities.Count > 0)
            {
                //AsyncOperationStateInfo completedState = new AsyncOperationStateInfo();
                //completedState.Value = AsyncOperationState.Completed;
                //Status canceledStatus = new Status(AsyncOperationStatus.Canceled);
                foreach (var entity in entities)
                {
                    try
                    {
                        //set state: completed; set status: canceled
                        var workflow = entity;
                        workflow["statecode"] = new OptionSetValue(3);
                        workflow["statuscode"] = new OptionSetValue((int)AsyncOperationStatus.Canceled);

                        //update status
                        crm.Update(workflow);
                    }
                    catch (Exception e)
                    {
                        if (!_notRethrowExceptionMessages.Any(message => e.Message.Contains(message)))
                        {
                            throw;
                        }
                    }
                }
            }
        }

        [Input("Workflow Name")]
        public InArgument<string> WorkflowName
        {
            get;
            set;
        }

        [Input("Cancel this workflow")]
        public InArgument<bool> CancelThisWorkflow
        {
            get;
            set;
        }
    }
    public enum AsyncOperationStatus
    {
        WaitingForResources = 0,
        Waiting = 10,
        InProgress = 20,
        Pausing = 21,
        Canceling = 22,
        Succeeded = 30,
        Failed = 31,
        Canceled = 32
    }
}
