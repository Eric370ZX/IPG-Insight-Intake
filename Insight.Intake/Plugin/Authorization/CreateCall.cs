using Insight.Intake.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Authorization
{
    public class CreateCall : PluginBase
    {
        private static readonly Dictionary<string, string> _callFields = new Dictionary<string, string>()
        {
            { PhoneCall.Fields.PhoneNumber, ipg_authorization.Fields.ipg_csrphone },
            { PhoneCall.Fields.CreatedBy, ipg_authorization.Fields.CreatedBy },
            { PhoneCall.Fields.Subject, ipg_authorization.Fields.ipg_callreference },
            { PhoneCall.Fields.RegardingObjectId, ipg_authorization.Fields.ipg_incidentid },
        };

        public CreateCall() : base(typeof(CreateCall))
        {
            RegisterEvent(
                PipelineStages.PreOperation,
                "Create",
                ipg_authorization.EntityLogicalName,
                OnCreate);
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            ipg_authorization target = pluginContext.PluginExecutionContext.GetTarget<ipg_authorization>();

            if (target.Attributes.Any(attr => _callFields.Any(x => x.Value == attr.Key)))
            {
                PhoneCall callRecord = new PhoneCall()
                {
                    From = new[]
                    {
                        new ActivityParty()
                        {
                            PartyId = target.CreatedBy
                        }
                    }
                };

                foreach (var field in _callFields)
                {
                    if (target.Contains(field.Value))
                        callRecord[field.Key] = target[field.Value];
                }

                callRecord.Id = pluginContext.OrganizationService.Create(callRecord);

                target.ipg_PhoneCallId = callRecord.ToEntityReference();
            }
        }
    }
}
