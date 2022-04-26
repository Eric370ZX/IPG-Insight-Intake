using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Insight.Intake.Consoles.InitAuthDueOrCreatedOn.Services
{
    public class IncidentService
    {
        private IOrganizationService _crmService;
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        private int _recordsPortionAmount = 25;

        public IncidentService(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public void ProcessRecordsWithAuthTasks()
        {
            var query = GetRecordsToProcessAuthTasksQuery();
            int pageRecordsAmount = 0, totalProcessedRecordsAmount = 0, totalRecordsAmount = 0;

            var grouppedRecords = _crmService.RetrieveMultiple(query).Entities
                .Select(x => x.ToEntity<Task>())
                .GroupBy(x => x.ipg_caseid);
            
            while (grouppedRecords.Any())
            {
                pageRecordsAmount = 0;
                totalRecordsAmount += grouppedRecords.Count();

                IEnumerable<Entity> recordsToUpdate = null;

                do
                {
                    recordsToUpdate = grouppedRecords
                        .Skip(pageRecordsAmount)
                        .Take(_recordsPortionAmount)
                        .Where(group => group.Any(record => record.ScheduledEnd != null && record.ScheduledEnd.HasValue))
                        .Select(group => new Incident()
                        {
                            Id = group.Key.Id,
                            ipg_soonestauthtaskid = group
                                .Where(record => record.ScheduledEnd != null)
                                .OrderBy(record => record.ScheduledEnd.Value)
                                .FirstOrDefault()?.ToEntityReference()
                        });

                    pageRecordsAmount += recordsToUpdate.Count();
                    totalProcessedRecordsAmount += recordsToUpdate.Count();

                    TimeSpan elapsedTime = UpdateMultipleRecords(recordsToUpdate);
                    _logger.Info($"{totalProcessedRecordsAmount} of {totalRecordsAmount} records have been processed in {elapsedTime}");
                } while (recordsToUpdate != null && recordsToUpdate.Any());

                query.PageInfo.PageNumber++;

                grouppedRecords = _crmService.RetrieveMultiple(query).Entities
                    .Select(x => x.ToEntity<Task>())
                    .GroupBy(x => x.ipg_caseid);
            }
        }

        public void ProcessWithoutAuthDueDates()
        {
            int pageRecordsAmount = 0, totalProcessedRecordsAmount = 0, totalRecordsAmount = 0;
            var query = GetWithoutAuthDateFieldQuery();
            var records = _crmService.RetrieveMultiple(query).Entities;

            while (records.Any())
            {
                pageRecordsAmount = 0;
                totalRecordsAmount += records.Count();

                IEnumerable<Entity> recordsToUpdate = null;

                do
                {
                    recordsToUpdate = records
                        .Skip(pageRecordsAmount)
                        .Take(_recordsPortionAmount)
                        .Select(r => r.ToEntity<Incident>())
                        .Where(r => r.CreatedOn != null && r.CreatedOn.HasValue)
                        .Select(r => new Incident()
                        {
                            Id = r.Id,
                            ipg_authdueorcreatedon = r.CreatedOn
                        });

                    pageRecordsAmount += recordsToUpdate.Count();
                    totalProcessedRecordsAmount += recordsToUpdate.Count();

                    TimeSpan elapsedTime = UpdateMultipleRecords(recordsToUpdate);

                    _logger.Info($"{totalProcessedRecordsAmount} of {totalRecordsAmount} records have been processed in {elapsedTime}");
                } while (recordsToUpdate != null && recordsToUpdate.Any());

                query.PageInfo.PageNumber++;
                records = _crmService.RetrieveMultiple(query).Entities;
            }
        }

        private QueryExpression GetWithoutAuthDateFieldQuery()
        {
            QueryExpression query = new QueryExpression(Incident.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(Incident.PrimaryIdAttribute, Incident.Fields.CreatedOn),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(Incident.Fields.ipg_authdueorcreatedon, ConditionOperator.Null),
                        new ConditionExpression(Incident.Fields.StateCode, ConditionOperator.Equal, (int)IncidentState.Active)
                    }
                },
                PageInfo =
                {
                    Count = 5000,
                    PageNumber = 1
                }
            };

            return query;
        }

        private IEnumerable<Entity> GetRecordsToProcess()
        {
            QueryExpression query = new QueryExpression(Incident.EntityLogicalName)
            {
                NoLock = true,
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(Incident.Fields.ipg_authdueorcreatedon, ConditionOperator.Null)
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
                    new LinkEntity(
                        Incident.EntityLogicalName, Task.EntityLogicalName,
                        Incident.PrimaryIdAttribute, Task.Fields.ipg_caseid, JoinOperator.LeftOuter)
                    {
                        EntityAlias = "task",
                        Columns = new ColumnSet(Task.PrimaryIdAttribute, Task.Fields.ScheduledEnd),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                            }
                        },
                        LinkEntities =
                        {
                            new LinkEntity(
                                Task.EntityLogicalName, Team.EntityLogicalName, 
                                Task.Fields.OwnerId, Team.PrimaryIdAttribute, JoinOperator.LeftOuter)
                            {
                                EntityAlias = "team"
                            },
                            new LinkEntity(
                                Task.EntityLogicalName, SystemUser.EntityLogicalName, 
                                Task.Fields.OwnerId, SystemUser.PrimaryIdAttribute, JoinOperator.LeftOuter)
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
                        }
                    }
                },
                PageInfo =
                {
                    Count = 4000
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }

        private QueryExpression GetRecordsToProcessAuthTasksQuery()
        {
            QueryExpression query = new QueryExpression(Task.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(Task.PrimaryIdAttribute, Task.Fields.ScheduledEnd, Task.Fields.ipg_caseid),
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                                new ConditionExpression(Task.Fields.ipg_caseid, ConditionOperator.NotNull)
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
                    new LinkEntity(
                        Task.EntityLogicalName, Incident.EntityLogicalName,
                        Task.Fields.ipg_caseid, Incident.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        EntityAlias = "incident",
                        LinkCriteria =
                        {
                            FilterOperator = LogicalOperator.Or,
                            Conditions =
                            {
                                new ConditionExpression($"{Incident.Fields.ipg_casestatusdisplayedid}name", ConditionOperator.Like, "Authorization%"),
                                new ConditionExpression(Incident.Fields.ipg_StateCode, ConditionOperator.Equal, (int)ipg_CaseStateCodes.Authorization),
                                new ConditionExpression(Incident.Fields.ipg_soonestauthtaskid, ConditionOperator.Null),
                            }
                        },
                        LinkEntities =
                        {
                            new LinkEntity(
                                Incident.EntityLogicalName, ipg_lifecyclestep.EntityLogicalName,
                                Incident.Fields.ipg_lifecyclestepid, ipg_lifecyclestep.PrimaryIdAttribute, JoinOperator.Inner)
                            {
                                LinkCriteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression($"{ipg_lifecyclestep.Fields.ipg_gateconfigurationid}name", ConditionOperator.Equal, "Gate 3")
                                    }
                                }
                            }
                        }
                    },
                    new LinkEntity(
                        Task.EntityLogicalName, Team.EntityLogicalName,
                        Task.Fields.OwnerId, Team.PrimaryIdAttribute, JoinOperator.LeftOuter)
                    {
                        EntityAlias = "team"
                    },
                    new LinkEntity(
                        Task.EntityLogicalName, SystemUser.EntityLogicalName,
                        Task.Fields.OwnerId, SystemUser.PrimaryIdAttribute, JoinOperator.LeftOuter)
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
                PageInfo =
                {
                    Count = 4000,
                    PageNumber = 1
                }
            };

            return query;
        }

        private TimeSpan UpdateMultipleRecords(IEnumerable<Entity> recordsToUpdate)
        {
            ExecuteMultipleRequest multipleUpdateRequest = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                }
            };

            multipleUpdateRequest.Requests.AddRange(recordsToUpdate
                .Select(r => new UpdateRequest()
                {
                    Target = r
                }));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var multipleUpdateResponse = _crmService.Execute(multipleUpdateRequest) as ExecuteMultipleResponse;
            sw.Stop();

            if (multipleUpdateResponse.IsFaulted)
            {
                foreach (var faultedResponse in multipleUpdateResponse.Responses.Where(r => r.Fault != null))
                {
                    UpdateRequest originalRequest = multipleUpdateRequest.Requests[faultedResponse.RequestIndex] as UpdateRequest;

                    string errorMessage =
                        $"Update request for {originalRequest.Target.LogicalName} ({originalRequest.Target.Id}) has failed: {faultedResponse.Fault.Message}";
                    _logger.Error(errorMessage);
                }
            }

            return sw.Elapsed;
        }
    }
}
