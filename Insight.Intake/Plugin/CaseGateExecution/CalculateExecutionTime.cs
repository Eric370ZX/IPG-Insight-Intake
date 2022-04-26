using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.CaseGateExecution
{
    public class CalculateExecutionTime : PluginBase
    {
        public CalculateExecutionTime() : base(typeof(CalculateExecutionTime))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_casegateexecution.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext context)
        {
            var target = context.Target<ipg_casegateexecution>();
            var preImage = context.PreImage<ipg_casegateexecution>();
            var entity = preImage == null ? target : target.Merge(preImage);

            if(entity.Contains(ipg_casegateexecution.Fields.ipg_StartDatetimePreExecution) 
                && entity.ipg_StartDatetimePreExecution != null
                && entity.Contains(ipg_casegateexecution.Fields.ipg_EndDatetimePreExecution)
                && entity.ipg_EndDatetimePreExecution != null)
            {
                target.ipg_PreGateExecution = (int)(((DateTime)entity.ipg_EndDatetimePreExecution.Value) - ((DateTime)entity.ipg_StartDatetimePreExecution.Value)).TotalMilliseconds;
            }

            if (entity.Contains(ipg_casegateexecution.Fields.ipg_StartDatetimeWTExecution)
                && entity.ipg_StartDatetimeWTExecution != null
                && entity.Contains(ipg_casegateexecution.Fields.ipg_EndDatetimeWTExecution)
                && entity.ipg_EndDatetimeWTExecution != null)
            {
                target.ipg_WTExecution = (int)(((DateTime)entity.ipg_EndDatetimeWTExecution.Value) - ((DateTime)entity.ipg_StartDatetimeWTExecution.Value)).TotalMilliseconds;
            }

            if (entity.Contains(ipg_casegateexecution.Fields.ipg_StartDatetimePostExecution)
                && entity.ipg_StartDatetimePostExecution != null
                && entity.Contains(ipg_casegateexecution.Fields.ipg_EndDatetimePostExecution)
                && entity.ipg_EndDatetimePostExecution != null)
            {
                target.ipg_PostGateExecution = (int)(((DateTime)entity.ipg_EndDatetimePostExecution.Value) - ((DateTime)entity.ipg_StartDatetimePostExecution.Value)).TotalMilliseconds;
            }

            if (entity.Contains(ipg_casegateexecution.Fields.ipg_StartDatetime)
                && entity.ipg_StartDatetime != null
                && entity.Contains(ipg_casegateexecution.Fields.ipg_EndDatetime)
                && entity.ipg_EndDatetime != null)
            {
                target.ipg_GateExecution = (int)(((DateTime)entity.ipg_EndDatetime.Value) - ((DateTime)entity.ipg_StartDatetime.Value)).TotalMilliseconds;
            }
        }
    }
}
