using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.ProcedureName
{
    public class CheckProcedureNameForDuplicate: PluginBase
    {
        private IOrganizationService _service;
        private OrganizationServiceContext _crmContext;

        public CheckProcedureNameForDuplicate() : base(typeof(CheckProcedureNameForDuplicate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_procedurename.EntityLogicalName, PostCreateOperationHandler);
        }

        private void PostCreateOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            _service = localPluginContext.OrganizationService;
            _crmContext = new OrganizationServiceContext(_service);

            var procedureName = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_procedurename>();

            if (!string.IsNullOrEmpty(procedureName.ipg_name))
            {
                var proceduresWithSameCode = (from procedure in _crmContext.CreateQuery<ipg_procedurename>()
                                            where procedure.ipg_name == procedureName.ipg_name
                                              select procedure).ToList();

                if (proceduresWithSameCode.Any())
                {
                    throw new Exception(string.Format("The Procedure Name {0} already exists!", procedureName.ipg_name));
                }
            }
        }
    }
}
