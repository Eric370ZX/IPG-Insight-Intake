using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class CalculateProcedureDate : PluginBase
    {

        public CalculateProcedureDate() : base(typeof(CalculateProcedureDate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, PreOperationCreateHandler);
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var tracingService = localPluginContext.TracingService;
                var entity = (Entity)context.InputParameters["Target"];

                string actualDosName = string.Empty;
                string surgeryDosName = string.Empty;
                if (string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName))
                {
                    actualDosName = nameof(ipg_referral.ipg_actualdos).ToLower();
                    surgeryDosName = nameof(ipg_referral.ipg_SurgeryDate).ToLower();
                }
                else if (string.Equals(entity.LogicalName, Incident.EntityLogicalName))
                {
                    actualDosName = nameof(Incident.ipg_ActualDOS).ToLower();
                    surgeryDosName = nameof(Incident.ipg_SurgeryDate).ToLower();
                }

                if (!string.IsNullOrEmpty(actualDosName) && !string.IsNullOrEmpty(surgeryDosName)) {
                    if (entity.Contains(actualDosName) || entity.Contains(surgeryDosName))
                    {
                        DateTime? procedureDate = null;
                        if(entity.Contains(actualDosName) && entity[actualDosName] != null)
                        {
                            procedureDate = (DateTime)entity[actualDosName];
                        }
                        else if(entity.Contains(surgeryDosName))
                        {
                            procedureDate = (DateTime)entity[surgeryDosName];
                        }
                        else if(context.MessageName == MessageNames.Update)
                        {
                            procedureDate = (DateTime)(service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(surgeryDosName))[surgeryDosName]);
                        }
                        entity[string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName) ? nameof(ipg_referral.ipg_ProcedureDate).ToLower() : nameof(Incident.ipg_ProcedureDateNew).ToLower()] = procedureDate;
                    }
                }

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}