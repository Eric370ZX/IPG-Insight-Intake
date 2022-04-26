using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Plugin.Gating.PostProcess
{
    public class FakeExecuteWorkflowRequest : IFakeMessageExecutor
    {
        public bool isExecuted = false;
        public bool CanExecute(OrganizationRequest request)
        {
            return request is ExecuteWorkflowRequest;
        }

        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            isExecuted = true;
            return new ExecuteWorkflowResponse();
            //throw new InvalidPluginExecutionException("Throwing an Invalid Plugin Execution Exception for test purposes");
        }

        public Type GetResponsibleRequestType()
        {
            return typeof(ExecuteWorkflowRequest);
        }
    }
}
