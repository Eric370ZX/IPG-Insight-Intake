using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Insight.Intake.Repositories
{
    public class TaskRepository
    {
        public static readonly string EntityName = Task.EntityLogicalName;

        private IOrganizationService _crmService;

        public TaskRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        /// <summary>
        /// Retrieves open tasks that related to a case and owner is assigned to Auth Team. Ordered by ScheduledEnd (Ascending)
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public IEnumerable<Entity> GetOpenAuthTasksByCase(Guid caseId, ColumnSet columns, int topCount = 100)
        {
            QueryExpression query = new QueryExpression(EntityName)
            {
                TopCount = topCount,
                ColumnSet = columns,
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.ipg_caseid, ConditionOperator.Equal, caseId),
                                new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                            }
                        },
                        new FilterExpression(LogicalOperator.Or)
                        {
                            Conditions =
                            {
                                new ConditionExpression("team", Team.PrimaryNameAttribute, ConditionOperator.Like, "Auth Spec%"),
                                new ConditionExpression("userteam", Team.PrimaryNameAttribute, ConditionOperator.Like, "Auth Spec%")
                            }
                        }
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(EntityName, Team.EntityLogicalName, Task.Fields.OwnerId, Team.PrimaryIdAttribute, JoinOperator.LeftOuter)
                    {
                        EntityAlias = "team"
                    },
                    new LinkEntity(EntityName, SystemUser.EntityLogicalName, Task.Fields.OwnerId, SystemUser.PrimaryIdAttribute, JoinOperator.LeftOuter)
                    {
                        LinkEntities =
                        {
                            new LinkEntity(
                                SystemUser.EntityLogicalName, "teammembership",
                                SystemUser.PrimaryIdAttribute, SystemUser.PrimaryIdAttribute, JoinOperator.LeftOuter)
                            {
                                LinkEntities =
                                {
                                    new LinkEntity(
                                        "teammembership", Team.EntityLogicalName,
                                        Team.PrimaryIdAttribute, Team.PrimaryIdAttribute, JoinOperator.LeftOuter)
                                    {
                                        EntityAlias = "userteam",
                                    }
                                }
                            }
                        }
                    }
                },
                Orders =
                {
                    new OrderExpression(Task.Fields.ScheduledEnd, OrderType.Ascending)
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }

        public IEnumerable<Entity> GetGenerateSubmitClaimOpenTasks(Guid caseId, ColumnSet columns)
        {
            string taskTypeAlias = "tasktype";

            QueryExpression query = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = columns,
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                                new ConditionExpression(Task.Fields.ipg_caseid, ConditionOperator.Equal, caseId)
                            }
                        },
                        new FilterExpression(LogicalOperator.Or)
                        {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.ipg_tasktypecode, ConditionOperator.Equal, (int)ipg_TaskType1.GenerateSubmitClaim),
                                new ConditionExpression(taskTypeAlias, ipg_tasktype.PrimaryNameAttribute, ConditionOperator.Like, $"%{TaskHelper.GenerateSubmitClaimSubject}%")
                            }
                        }
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        Task.EntityLogicalName, ipg_tasktype.EntityLogicalName, 
                        Task.Fields.ipg_tasktypeid, ipg_tasktype.PrimaryIdAttribute, JoinOperator.LeftOuter)
                    {
                        EntityAlias = taskTypeAlias,
                        Columns = new ColumnSet(ipg_tasktype.PrimaryNameAttribute)
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }

        public IEnumerable<Entity> GetOpenMissingTissueRequestForm(Guid caseId, ColumnSet columns, int topCount = 5000)
        {
            string taskTypeAlias = "taskTypeMissingTissueRequestForm";

            QueryExpression query = new QueryExpression(Task.EntityLogicalName)
            {
                TopCount = topCount,
                ColumnSet = columns,
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.ipg_caseid, ConditionOperator.Equal, caseId),
                                new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                            }
                        },
                        new FilterExpression(LogicalOperator.Or)
                        {
                            Conditions =
                            {
                                new ConditionExpression(taskTypeAlias, ipg_tasktype.PrimaryIdAttribute, ConditionOperator.NotNull),
                                new ConditionExpression(Task.Fields.ipg_tasktypecode, ConditionOperator.Equal, (int)ipg_TaskType1.MissinginformationTissueRequestForm)
                            }
                        }
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        Task.EntityLogicalName, ipg_tasktype.EntityLogicalName, 
                        Task.Fields.ipg_tasktypeid, ipg_tasktype.PrimaryIdAttribute, JoinOperator.LeftOuter)
                    {
                        EntityAlias = taskTypeAlias,
                        Columns = new ColumnSet(false),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_tasktype.Fields.ipg_name, ConditionOperator.Like, $"%Missing Tissue Request Form%")
                            }
                        }
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }
    }
}
