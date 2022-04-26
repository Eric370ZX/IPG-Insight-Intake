using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class AddNewBenefitNoteAndAuthorizationNote: PluginBase
    {
        Dictionary<string, string> notesMapping = new Dictionary<string, string>() {
            { "ipg_newauthorizationnote", "ipg_authorizationnotes" },
            { "ipg_newbenefitsnote", "ipg_benefitsnotes" }
        };

        public AddNewBenefitNoteAndAuthorizationNote() : base(typeof(AddNewBenefitNoteAndAuthorizationNote))
        {
            RegisterEvent(PipelineStages.PreOperation, "Create", Incident.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, "Update", Incident.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;

            var target = ((Entity)context.InputParameters["Target"]);

            Entity preImage = null;
            if (context.PreEntityImages != null && context.PreEntityImages.Contains("Image"))
            {
                preImage = (Entity)context.PreEntityImages["Image"];
            }

            foreach (var item in notesMapping) {
                if (target.Contains(item.Key))
                {
                    var authorizationNotes = preImage != null ? preImage.GetAttributeValue<string>(item.Value) : string.Empty;
                    if (string.IsNullOrEmpty(authorizationNotes))
                    {
                        target[item.Value] = target.GetAttributeValue<string>(item.Key);
                    }
                    else
                    {
                        target[item.Value] = authorizationNotes + System.Environment.NewLine + target.GetAttributeValue<string>(item.Key);
                    }
                    target[item.Key] = string.Empty;
                }
            }
        }
    }
}
