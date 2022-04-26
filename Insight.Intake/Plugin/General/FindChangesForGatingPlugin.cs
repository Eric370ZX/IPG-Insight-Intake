using Insight.Intake.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace Insight.Intake.Plugin.General
{
    [Obsolete]
    public class FindChangesForGatingPlugin : PluginBase
    {
        public FindChangesForGatingPlugin() : base(typeof(FindChangesForGatingPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {

        }
    }       
}
