namespace Insight.Intake.Plugin.Contact
{
    public class SetFullName : PluginBase
    {
        public SetFullName() : base(typeof(SetFullName))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Intake.Contact.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localContext)
        {
            var contact = localContext.Target<Intake.Contact>();
            var preContact = localContext.PluginExecutionContext.InputParameters.Contains("PreImage")
                             ? localContext.PreImage<Intake.Contact>()
                             : null;
            var firstName = contact?.FirstName ?? preContact?.FirstName;
            var middleName = contact?.MiddleName ?? preContact?.MiddleName;
            var lastName = contact?.LastName ?? preContact?.LastName;
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                contact.ipg_fullname = $"{lastName}, {firstName} {middleName}".Trim();
            }
        }
    }
}
