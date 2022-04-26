namespace Insight.Intake.Plugin.Case
{
    public class AddSecondCarrier : PluginBase
    {
        public AddSecondCarrier() : base(typeof(AddSecondCarrier))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_CaseCarrierAddingParameters.EntityLogicalName, PostOperationCreateHandler);
        }


        void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.Target<ipg_CaseCarrierAddingParameters>();
            if (target == null)
            {
                localPluginContext.TracingService.Trace("Target is empty. Exit");
                return;
            }

            localPluginContext.TracingService.Trace("Updating the case");
            var incidentUpdate = new Incident
            {
                Id = target.ipg_CaseId.Id,
                ipg_SecondaryCarrierId = target.ipg_CarrierId,
                ipg_SecondaryMemberIdNumber = target.ipg_MemberIdNumber,
                ipg_SecondaryCarrierGroupIdNumber = target.ipg_GroupNumber
            };
            service.Update(incidentUpdate);
        }
    }
}
