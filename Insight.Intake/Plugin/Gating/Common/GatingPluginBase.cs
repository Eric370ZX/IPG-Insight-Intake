using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Gating.Common
{
    public abstract class GatingPluginBase : PluginBase
    {
        internal IOrganizationService crmService {  get; private set; }
        internal ITracingService tracingService { get; private set; }
        internal EntityReference targetRef { get; private set; }

        public GatingPluginBase(string actionName) : base(typeof(GatingPluginBase))
        {
            RegisterEvent(actionName);
        }

        public GatingPluginBase(LocalPluginContext localPluginContext, string actionName) : base(typeof(GatingPluginBase))
        {
            RegisterEvent(actionName);
            InitProperties(localPluginContext);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            InitProperties(localPluginContext);

            var gateManager = new GateManager(crmService, tracingService, targetRef);
            var result = Validate(gateManager, localPluginContext);
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["PortalNote"] = result.PortalNote;
            localPluginContext.PluginExecutionContext.OutputParameters["CaseNote"] = result.CaseNote;
            localPluginContext.PluginExecutionContext.OutputParameters["NegativeMessage"] = result.CustomMessage;
            localPluginContext.PluginExecutionContext.OutputParameters["TaskSubject"] = result.TaskSubject;
            localPluginContext.PluginExecutionContext.OutputParameters["TaskDescripton"] = result.TaskDescripton;
            localPluginContext.PluginExecutionContext.OutputParameters["GatingOutcome"] = result.GatingOutcome;
            localPluginContext.PluginExecutionContext.OutputParameters["CodeOutput"] = result.CodeOutput;            
        }
        public abstract GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null);

        private void RegisterEvent(string actionName)
        {
            RegisterEvent(PipelineStages.PostOperation, actionName, null, PostOperationHandler);
        }

        private void InitProperties(LocalPluginContext localPluginContext)
        {
            this.crmService = localPluginContext.OrganizationService;
            this.tracingService = localPluginContext.TracingService;
            this.targetRef = localPluginContext.TargetRef();
        }
    }
}
