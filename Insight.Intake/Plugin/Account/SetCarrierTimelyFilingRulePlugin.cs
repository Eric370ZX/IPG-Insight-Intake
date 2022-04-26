namespace Insight.Intake.Plugin.Account
{
    public class SetCarrierTimelyFilingRulePlugin : PluginBase
    {
        public SetCarrierTimelyFilingRulePlugin() : base(typeof(SetCarrierTimelyFilingRulePlugin))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Intake.Account.EntityLogicalName, PreOperationCreateHandler);
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Intake.Account>();
            if (target.CustomerTypeCodeEnum != Account_CustomerTypeCode.Carrier)
            {
                return;
            }
            if (target.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp || target.Name == "BCBS-Anthem-GA")
            {
                target.ipg_timelyfilingrule = 90;
            }
            else if (target.Name == "BCBS-Anthem-CA")
            {
                target.ipg_timelyfilingrule = 150;
            }
            else
            {
                target.ipg_timelyfilingrule = 180;
            }
        }
    }
}
