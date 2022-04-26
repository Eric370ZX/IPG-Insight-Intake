using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.Managers
{
    public class UserManager
    {
        private TeamRepository _teamRepository;

        private IOrganizationService _crmService;

        public UserManager(IOrganizationService crmService)
        {
            _crmService = crmService;

            _teamRepository = new TeamRepository(crmService);
        }

        public bool IsUserAssociatedWithAuthTeam(Guid userId)
        {
            ColumnSet teamColumns = new ColumnSet(Team.PrimaryNameAttribute, Team.PrimaryIdAttribute);

            return _teamRepository
                .GetByUser(userId, teamColumns)
                .Select(x => x.ToEntity<Team>())
                .Any(x => x.Name != null && x.Name.ToLower().Contains("auth"));
        }

        internal bool IsAuth(EntityReference ownerRef)
        {
            if (ownerRef != null)
            {
                if (ownerRef.LogicalName == SystemUser.EntityLogicalName)
                {
                    return IsUserAssociatedWithAuthTeam(ownerRef.Id);
                }
                else
                {
                    Team team = _crmService.Retrieve<Team>(ownerRef.Id, new ColumnSet(Team.PrimaryNameAttribute));

                    return !string.IsNullOrWhiteSpace(team.Name) && team.Name.ToLower().Contains("auth spec");
                }
            }

            return false;
        }
    }
}
