using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;


namespace Insight.Intake.Plugin.FacilityCPT
{
    //Validator for facility CPT
    internal class FaciltyCPTValidator
    {
        private IOrganizationService service;
        internal FaciltyCPTValidator(IOrganizationService service)
        {
            this.service = service;
        }
        internal void ValidateForOverlappingFacilityCPT(ipg_facilitycpt facilityCPT)
        {
            var queryExpression = new QueryExpression(ipg_facilitycpt.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_facilitycptId),
                                          LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_CptCodeId),
                                          LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_name)),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Filters = {
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_facilitycpt>.Property(e => e.ipg_FacilityId), ConditionOperator.Equal, facilityCPT.ipg_FacilityId.Id),
                                new ConditionExpression(LogicalNameof<ipg_facilitycpt>.Property(e => e.ipg_CptCodeId), ConditionOperator.Equal, facilityCPT.ipg_CptCodeId.Id),
                                new ConditionExpression(LogicalNameof<ipg_facilitycpt>.Property(e => e.ipg_facilitycptId), ConditionOperator.NotEqual, facilityCPT.ipg_facilitycptId),
                                new ConditionExpression(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, facilityCPT.ipg_ExpirationDate),
                                new ConditionExpression(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, facilityCPT.ipg_EffectiveDate)
                            }
                        }
                    }

                }
            };

            //Check if there is any overlapping cpt under the same facility
            var overlappingRules = service.RetrieveMultiple(queryExpression);
            if (overlappingRules.Entities.Any())
            {
                throw new Exception($"There is a overlapping CPT " +
                    $"{overlappingRules.Entities.FirstOrDefault().ToEntity<ipg_facilitycpt>().ipg_CptCodeId.Name} " +
                    $"in this Facility.");
            }
        }
    }
}
