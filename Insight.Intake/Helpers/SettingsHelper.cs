using Insight.Intake.Models.Settings;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Helpers
{
    public class SettingsHelper
    {
        private IOrganizationService _crmService;

        public SettingsHelper(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public SlaTaskSettings GetSlaTaskSettings()
        {
            var records = _crmService.RetrieveMultiple(new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_name", "ipg_value", "ipg_type"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("ipg_name", ConditionOperator.Like, "sla-due-date-offset%")
                    }
                }
            }).Entities;

            var createReferral = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-create-referral")
                .ToEntity<ipg_globalsetting>();
            var decisionRetroCase = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-decision-retro-case")
                .ToEntity<ipg_globalsetting>();
            var decisionStandardCase = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-decision-standard-case")
                .ToEntity<ipg_globalsetting>();
            var decisionStatCase = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-decision-stat-case")
                .ToEntity<ipg_globalsetting>();
            var decisionUrgentCase = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-decision-urgent-case")
                .ToEntity<ipg_globalsetting>();
            var generatePo = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-generate-po")
                .ToEntity<ipg_globalsetting>();
            var payProvider = records
                .FirstOrDefault(x => x.ToEntity<ipg_globalsetting>().ipg_name == "sla-due-date-offset-pay-provider")
                .ToEntity<ipg_globalsetting>();

            return new SlaTaskSettings()
            {
                CreateRefferalDueDateOffset = createReferral.ipg_Type == "hours" ? 
                    Convert.ToInt32(createReferral.ipg_value) : 
                    Convert.ToInt32(createReferral.ipg_value) * 24,
                DecisionRetroCaseDueDateOffset = decisionRetroCase.ipg_Type == "hours" ?
                    Convert.ToInt32(decisionRetroCase.ipg_value) :
                    Convert.ToInt32(decisionRetroCase.ipg_value) * 24,
                DecisionStandardCaseDueDateOffset = decisionStandardCase.ipg_Type == "hours" ?
                    Convert.ToInt32(decisionStandardCase.ipg_value) :
                    Convert.ToInt32(decisionStandardCase.ipg_value) * 24,
                DecisionStatCaseDueDateOffset = decisionStatCase.ipg_Type == "hours" ?
                    Convert.ToInt32(decisionStatCase.ipg_value) :
                    Convert.ToInt32(decisionStatCase.ipg_value) * 24,
                DecisionUrgentCaseDueDateOffset = decisionUrgentCase.ipg_Type == "hours" ?
                    Convert.ToInt32(decisionUrgentCase.ipg_value) :
                    Convert.ToInt32(decisionUrgentCase.ipg_value) * 24,
                GeneratePoDueDateOffset = generatePo.ipg_Type == "hours" ?
                    Convert.ToInt32(generatePo.ipg_value) :
                    Convert.ToInt32(generatePo.ipg_value) * 24,
                PayProviderDueDateOffset = payProvider.ipg_Type == "hours" ?
                    Convert.ToInt32(payProvider.ipg_value) :
                    Convert.ToInt32(payProvider.ipg_value) * 24
            };
        }

        public IEnumerable<string> GetTissueRequestFormCpts()
        {
            Entity record = _crmService.RetrieveMultiple(new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_value"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("ipg_name", ConditionOperator.Equal, "TissueRequestForm_CPTCodes")
                    }
                }
            }).Entities.FirstOrDefault();

            if (record is ipg_globalsetting setting && !string.IsNullOrWhiteSpace(setting.ipg_value))
            {
                return setting.ipg_value.Split(',');
            }

            throw new InvalidPluginExecutionException("Can't find settings for Tissue Request Form CPT Codes (TissueRequestForm_CPTCodes)");
        }
    }
}
