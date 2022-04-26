using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.CptCode
{
    public class CheckCPTCodeForDuplicate: PluginBase
    {
        private IOrganizationService _service;
        private OrganizationServiceContext _crmContext;

        public CheckCPTCodeForDuplicate() : base(typeof(CheckCPTCodeForDuplicate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_cptcode.EntityLogicalName, PostCreateOperationHandler);
        }

        private void PostCreateOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            _service = localPluginContext.OrganizationService;
            _crmContext = new OrganizationServiceContext(_service); 

            var cptCode = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_cptcode>();

            if (!string.IsNullOrEmpty(cptCode.ipg_cptcode1)) {
                var cptCodesWithSameCode = (from cpt in _crmContext.CreateQuery<ipg_cptcode>()
                                            where cpt.ipg_cptcode1 == cptCode.ipg_cptcode1
                                            select cpt).ToList();

                if (cptCodesWithSameCode.Any()) {
                    throw new Exception(string.Format("The CPT Code {0} already exists!", cptCode.ipg_cptcode1));
                }
            }
        }
    }
}
