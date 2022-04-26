using System;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class SetCaseCriticalFieldsLastChangeDate : PluginBase
    {
        public SetCaseCriticalFieldsLastChangeDate() : base(typeof(SetCaseCriticalFieldsLastChangeDate))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_casepartdetail.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            var service = context.OrganizationService;
            var partDetail = context.PostImage<ipg_casepartdetail>();

            if (partDetail?.ipg_caseid?.Id != null)
                service.Update(new Incident {
                    Id = partDetail.ipg_caseid.Id,
                    ipg_criticalfieldslastchangedate = partDetail.ModifiedOn,
                    ipg_changedcriticalfield = "Parts"
                });
        }
    }
}