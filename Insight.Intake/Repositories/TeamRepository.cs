using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Repositories
{
    public class TeamRepository
    {
        private IOrganizationService _crmService;

        public static readonly string EntityName = Team.EntityLogicalName;

        public TeamRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public IEnumerable<Entity> GetByUser(Guid userId, ColumnSet columns)
        {
            QueryExpression query = new QueryExpression(EntityName)
            {
                ColumnSet = columns,
                LinkEntities =
                {
                    new LinkEntity(
                        Team.EntityLogicalName, "teammembership", 
                        Team.PrimaryIdAttribute, Team.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(SystemUser.PrimaryIdAttribute, ConditionOperator.Equal, userId)
                            }
                        }
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }

        public Team GetByName(string name, ColumnSet columns)
        {
            QueryExpression query = new QueryExpression(EntityName)
            {
                ColumnSet = columns,
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(Team.Fields.Name, ConditionOperator.Equal, name)
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Team>();
        }
    }
}