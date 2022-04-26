using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.AccountUser
{
    public class CheckAccountUserOnDuplicatePlugin : PluginBase
    {
        public CheckAccountUserOnDuplicatePlugin() : base(typeof(CheckAccountUserOnDuplicatePlugin))
        {
            RegisterEvent(PipelineStages.PreOperation, "Create", "ipg_accountuser", PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, "Update", "ipg_accountuser", PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext context)
        {
            var target = context.Target<ipg_accountuser>();
            var preImage = context.PreImage<ipg_accountuser>();
            var entityData = preImage == null ? target : target.Merge(preImage);

            if (entityData.StateCode == ipg_accountuserState.Active)
            {
                var crmContext = new OrganizationServiceContext(context.OrganizationService);

                var duplicateId = (from au in crmContext.CreateQuery<ipg_accountuser>()
                                   where au.ipg_userid.Equals(entityData.ipg_userid)
                                          && au.ipg_accountid.Equals(entityData.ipg_accountid)
                                          && au.ipg_rolecode.Equals(entityData.ipg_rolecode)
                                          && au.StateCode.Equals(ipg_accountuserState.Active)
                                          && au.ipg_accountuserId != target.Id
                                   select au.ipg_accountuserId).FirstOrDefault();

                if (duplicateId != null)
                {
                    throw new Exception("This relationship already exist!");
                }
            }
        }
    }
}
