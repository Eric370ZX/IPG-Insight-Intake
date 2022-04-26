using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Client;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.StatementGenerationTask
{
    public class UpdateLastStatementType : PluginBase
    {

        public UpdateLastStatementType() : base(typeof(UpdateLastStatementType))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_statementgenerationtask.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var preImage = localPluginContext.PreImage<ipg_statementgenerationtask>();
            if (preImage.StatusCode.Value != (int)ipg_statementgenerationtask_StatusCode.Completed)
            {
                var target = localPluginContext.Target<ipg_statementgenerationtask>();
                if (target.StatusCode.Value == (int)ipg_statementgenerationtask_StatusCode.Completed)
                {
                    var statement = service.Retrieve(target.LogicalName, target.Id, new ColumnSet(ipg_statementgenerationtask.Fields.ipg_caseid,
                        ipg_statementgenerationtask.Fields.ipg_eventid)).ToEntity<ipg_statementgenerationtask>();

                    var incident = new Incident();
                    incident.Id = statement.ipg_caseid.Id;
                    incident.ipg_LastStatementType = statement.ipg_eventid;
                    service.Update(incident);
                }
            }
        }        
    }
}