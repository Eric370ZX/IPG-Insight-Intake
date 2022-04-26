using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating.PreProcess
{
    public class Gate8 : PluginBase
    {
        public Gate8() : base(typeof(Gate8))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPreProcessGate8", null, GatingProProcessHandler);
        }
        private void GatingProProcessHandler(LocalPluginContext localPluginContext)
        {
            var claims = GetClaims(localPluginContext);
            var isCaseLocked = IsCaseLocked(localPluginContext);
            var hasClaimGenerationOverrides = HasClaimGenerationOverride(localPluginContext);
            var isGateFailed = claims.Any() && !isCaseLocked && !hasClaimGenerationOverrides;
            var hasClaimGenerationOverridesNoClaim = HasClaimGenerationOverrideNoClaim(localPluginContext);
            ReopenTasks(localPluginContext, localPluginContext.TargetRef(), hasClaimGenerationOverridesNoClaim);
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = !isGateFailed;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "Incorrect claim generation override";
        }

        private bool HasClaimGenerationOverride(LocalPluginContext ctx)
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='ipg_claimgenerationoverride'>
                            <attribute name='ipg_claimgenerationoverrideid' />
                            <attribute name='ipg_id' />
                            <attribute name='createdon' />
                            <attribute name='modifiedon' />
                            <order attribute='ipg_id' descending='false' />
                            <filter type='and'>
                              <condition attribute='ipg_caseid' operator='eq' value='{ctx.TargetRef().Id}' />
                            </filter>
                          </entity>
                        </fetch>";
            var claimsOverrides = ctx.SystemOrganizationService.RetrieveMultiple(new FetchExpression(fetch))
                .Entities
                .Select(p => p.ToEntity<ipg_claimgenerationoverride>());
            if (!claimsOverrides.Any())
            {
                return false;
            }
            var targetCase = ctx.SystemOrganizationService.Retrieve(ctx.TargetRef().LogicalName, ctx.TargetRef().Id, new ColumnSet(Incident.Fields.ipg_caseunlockedon)).ToEntity<Incident>();

            return claimsOverrides.Any(p => p.ModifiedOn >= targetCase.ipg_caseunlockedon);
        }

        private bool HasClaimGenerationOverrideNoClaim(LocalPluginContext ctx)
        {
            var query = new QueryExpression(ipg_claimgenerationoverride.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_claimgenerationoverride.Fields.ipg_caseid, ConditionOperator.Equal, ctx.TargetRef().Id),
                        new ConditionExpression(ipg_claimgenerationoverride.Fields.ipg_claimtogenerate,  ConditionOperator.Equal, (int)ipg_claimgenerationoverride_ipg_claimtogenerate.NoClaim)
                    }
                },
            };
            var ec = ctx.SystemOrganizationService.RetrieveMultiple(query);
            return (ec.Entities.Count > 0);
        }

        private bool IsCaseLocked(LocalPluginContext ctx)
        {
            var targetCase = ctx.SystemOrganizationService.Retrieve(ctx.TargetRef().LogicalName, ctx.TargetRef().Id, new ColumnSet(Incident.Fields.ipg_islocked)).ToEntity<Incident>();
            return targetCase.ipg_islocked ?? false;
        }

        private IEnumerable<Entity> GetClaims(LocalPluginContext ctx)
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='invoice'>
                            <attribute name='name' />
                            <attribute name='invoiceid' />
                            <order attribute='name' descending='false' />
                            <filter type='and'>
                              <condition attribute='ipg_caseid' operator='eq' value='{ctx.TargetRef().Id}' />
                            </filter>
                          </entity>
                        </fetch>";
            var result = ctx.SystemOrganizationService.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }

        private void ReopenTasks(LocalPluginContext ctx, EntityReference caseRef, bool hasClaimGenerationOverrides)
        {
            if (!hasClaimGenerationOverrides)
            {
                return;
            }
            var crmContext = new OrganizationServiceContext(ctx.OrganizationService);

            var statementGenerationTask = (from task in crmContext.CreateQuery<ipg_statementgenerationtask>()
                                           where task.ipg_caseid.Equals(caseRef)
                                                && task.StatusCode.Equals(ipg_statementgenerationtask_StatusCode.Canceled)
                                           orderby task.ipg_StartDate descending
                                           select task).FirstOrDefault();
            if (statementGenerationTask != null)
            {
                ctx.SystemOrganizationService.Update(
                    new ipg_statementgenerationtask()
                    {
                        Id = statementGenerationTask.Id,
                        StateCode = ipg_statementgenerationtaskState.Active,
                        StatusCodeEnum = ipg_statementgenerationtask_StatusCode.Open
                    });
            }

            var patientOutreachTask = ctx.SystemOrganizationService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1,
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Filters = {
                        new FilterExpression(LogicalOperator.And) {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.RegardingObjectId,  ConditionOperator.Equal, caseRef.Id)
                            }
                        },
                        new FilterExpression(LogicalOperator.Or) {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.StatusCode,  ConditionOperator.Equal, (int)Task_StatusCode.Canceled),
                                new ConditionExpression(Task.Fields.StatusCode,  ConditionOperator.Equal, (int)Task_StatusCode.Cancelled)
                            }
                        },
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(Task.EntityLogicalName, ipg_taskcategory.EntityLogicalName, ipg_taskcategory.PrimaryIdAttribute, Task.Fields.ipg_taskcategoryid, JoinOperator.Inner)
                    {
                        LinkCriteria = new FilterExpression(LogicalOperator.Or)
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_taskcategory.Fields.ipg_name, ConditionOperator.Equal, Helpers.Constants.TaskCategoryNames.PatientOutreach),
                            }
                        }
                    }
                },
                Orders =
                {
                    new OrderExpression(Task.Fields.CreatedOn, OrderType.Descending)
                }
            }).Entities.FirstOrDefault();
            if (patientOutreachTask != null)
            {
                ctx.OrganizationService.Update(new Task()
                {
                    Id = patientOutreachTask.Id,
                    StateCode = TaskState.Open,
                    StatusCode = new OptionSetValue((int)Task_StatusCode.InProgress),
                    ipg_taskreason = null
                });
            }
        }
    }
}
