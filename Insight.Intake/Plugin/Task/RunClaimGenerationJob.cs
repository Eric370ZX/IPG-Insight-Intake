using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class RunClaimGenerationJob : PluginBase
    {
        //private readonly string _appUrl = "https://prod-12.eastus.logic.azure.com:443/workflows/760e9ac2bca84878be447fe547fe91eb/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=qZPNel6sdLSMeV85mzL84jPgTwxdMOtRg_GAVJAWxBU";
        public RunClaimGenerationJob() : base(typeof(RunClaimGenerationJob))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsRunClaimGenerationJob", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var url = (from gc in crmContext.CreateQuery<ipg_globalsetting>()
                       where gc.ipg_name == Constants.Settings.GenerateClaimLogicApp
                       select gc.ipg_value).FirstOrDefault();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = 0;
            request.Method = "POST";

            HttpWebResponse responsee = (HttpWebResponse)request.GetResponse();
            if (responsee.StatusCode == HttpStatusCode.OK || responsee.StatusCode == HttpStatusCode.Accepted)
            {
                localPluginContext.PluginExecutionContext.OutputParameters["HasErrors"] = false;
            }
            else
            {
                localPluginContext.PluginExecutionContext.OutputParameters["HasErrors"] = true;
                localPluginContext.PluginExecutionContext.OutputParameters["Message"] = responsee.StatusDescription;
            }
        }
    }
}
