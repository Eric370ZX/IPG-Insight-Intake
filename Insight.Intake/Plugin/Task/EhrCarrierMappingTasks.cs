using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Common;
using Insight.Intake.Plugin.Common.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class EhrCarrierMappingTasks : PluginBase
    {
        private IHttpClientService _httpClientService;

        public EhrCarrierMappingTasks() : base(typeof(EhrCarrierMappingTasks))
        {
            RegisterEvents();
        }

        /// <summary>
        /// Constructor for unit tests
        /// </summary>
        /// <param name="httpClientService"></param>
        public EhrCarrierMappingTasks(IHttpClientService httpClientService) : base(typeof(EhrCarrierMappingTasks))
        {
            _httpClientService = httpClientService;

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void InitializeServices()
        {
            if (_httpClientService == null)
            {
                _httpClientService = new HttpClientService();
            }
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var organizationService = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace("Initializing services");
            InitializeServices();

            tracingService.Trace("Getting Target Task");
            var targetTask = localPluginContext.Target<Task>();

            if (targetTask.StateCode == TaskState.Completed)
            {
                tracingService.Trace("Getting Task PostImage");
                var taskPostImage = localPluginContext.PostImage<Task>();
                if (taskPostImage != null
                    && taskPostImage.ipg_tasktypeid != null
                    && taskPostImage.RegardingObjectId != null && taskPostImage.RegardingObjectId.LogicalName == ipg_ehrcarriermap.EntityLogicalName)
                {
                    tracingService.Trace("Retrieving Task Type");
                    var taskType = organizationService.Retrieve(ipg_tasktype.EntityLogicalName, taskPostImage.ipg_tasktypeid.Id,
                        new ColumnSet(ipg_tasktype.Fields.ipg_typeid)).ToEntity<ipg_tasktype>();
                    if (taskType?.ipg_typeid == (int)TaskTypeIds.UNKNOWN_FACILITY_CARRIER_COMBINATION_FROM_EHR)
                    {
                        tracingService.Trace("Retrieving EHR Carrier Mapping");
                        var mapping = organizationService.Retrieve(ipg_ehrcarriermap.EntityLogicalName, taskPostImage.RegardingObjectId.Id, new ColumnSet(
                                                    ipg_ehrcarriermap.Fields.ipg_carrierid
                                                        )).ToEntity<Intake.ipg_ehrcarriermap>();
                        if (mapping.ipg_carrierid == null)
                        {
                            throw new InvalidPluginExecutionException("Carrier must be specified in the mapping relationship to complete the task");
                        }

                        ResetEhrStatusAsync(organizationService, tracingService, mapping.ipg_ehrcarriermapId.Value).Wait();

                        tracingService.Trace("Updating Mapping statuses");
                        if(mapping.ipg_StatusEnum != ipg_EHRCarrierMappingStatuses.Complete
                            || mapping.ipg_take != true)
                        {
                            mapping.ipg_StatusEnum = ipg_EHRCarrierMappingStatuses.Complete;
                            mapping.ipg_take = true;
                            organizationService.Update(mapping);
                        }
                    }
                }
            }
        }

        private async System.Threading.Tasks.Task ResetEhrStatusAsync(IOrganizationService organizationService, ITracingService tracingService, Guid carrierMappingId)
        {
            tracingService.Trace("Getting Global Setting");
            string ehrResubmitUrl = D365Helpers.GetGlobalSettingValueByKey(organizationService, Constants.Settings.EHRResubmitURL);
            if (string.IsNullOrWhiteSpace(ehrResubmitUrl))
            {
                throw new Exception($"Global setting {Constants.Settings.EHRResubmitURL} is not configured");
            }

            tracingService.Trace("Resetting EHR statuses");
            ehrResubmitUrl += "&CarrierMappingId=" + Uri.EscapeDataString(carrierMappingId.ToString());
            await _httpClientService.PostAsync(ehrResubmitUrl).ConfigureAwait(false);
        }
    }
}
