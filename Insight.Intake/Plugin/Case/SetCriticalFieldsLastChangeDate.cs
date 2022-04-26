using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Case
{
    public class SetCriticalFieldsLastChangeDate : PluginBase
    {
        public SetCriticalFieldsLastChangeDate() : base(typeof(SetCriticalFieldsLastChangeDate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, PreOperationHandler);
        }

        void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = (Entity)localPluginContext.PluginExecutionContext.InputParameters["Target"];
            var criticalFields = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string> ("ipg_actualdos", "Actual DOS"),
                new KeyValuePair<string, string> ("ipg_carrierid", "Carrier Name"),
                new KeyValuePair<string, string> ("ipg_cptcodeid1", "CPT Code"),
                new KeyValuePair<string, string> ("ipg_cptcodeid2", "CPT Code"),
                new KeyValuePair<string, string> ("ipg_cptcodeid3", "CPT Code"),
                new KeyValuePair<string, string> ("ipg_cptcodeid4", "CPT Code"),
                new KeyValuePair<string, string> ("ipg_cptcodeid5", "CPT Code"),
                new KeyValuePair<string, string> ("ipg_cptcodeid6", "CPT Code"),
                new KeyValuePair<string, string> ("ipg_memberidnumber", "Member ID"),
            };

            var changedcriticalfield = "";

            foreach (var field in criticalFields)
            {
                if (target.Contains(field.Key))
                {
                    changedcriticalfield += string.IsNullOrEmpty(changedcriticalfield) ?  field.Value : $", {field.Value}";
                }
            }

            var incident = localPluginContext.Target<Incident>();

            incident.ipg_criticalfieldslastchangedate = DateTime.UtcNow;
            incident.ipg_changedcriticalfield = changedcriticalfield;
        }
    }
}