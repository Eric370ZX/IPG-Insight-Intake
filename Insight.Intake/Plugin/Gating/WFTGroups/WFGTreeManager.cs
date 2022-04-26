using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.WFTGroups
{
    public class WFGTreeManager
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public WFGTreeManager(IOrganizationService service, ITracingService tracingService)
        {
            this._service = service;
            this._tracingService = tracingService;
        }
        public List<WFGroup> GetGatingTreeV3(IEnumerable<ipg_gateconfigurationdetail> gateConfigurationDetails)
        {
            var gatingTree = new List<WFGroup>();
            foreach (var record in gateConfigurationDetails.Where(e => e.ipg_WorkflowTaskId != null).Select(e => new { workflowTask = e.ipg_WorkflowTaskId , gateConfigurationDetail = e }))
            {
                var workflowTask = _service.Retrieve(record.workflowTask.LogicalName, record.workflowTask.Id, new ColumnSet(ipg_workflowtask.Fields.ipg_WFTaskGroupId)).ToEntity<ipg_workflowtask>();
                var currentNode = GetInsertGroupFromTree(ref gatingTree, workflowTask.ipg_WFTaskGroupId);
                currentNode.GateDetails.Add(record.gateConfigurationDetail);
            }
            foreach (var gateConfigurationDetail in gateConfigurationDetails.Where(e => e.ipg_WorkflowTaskId == null))
            {
                var currentNode = GetInsertGroupFromTree(ref gatingTree, null);
                currentNode.GateDetails.Add(gateConfigurationDetail);
            }
            return gatingTree;
        }
        private WFGroup GetInsertGroupFromTree(ref List<WFGroup> gatingTree, EntityReference wftGroupRef)
        {
            var currentNode = gatingTree.FirstOrDefault(p => (wftGroupRef == null && p.Group == null) || wftGroupRef?.Id == p.Group?.Id);
            if (currentNode == null)
            {
                currentNode = new WFGroup();
                currentNode.GateDetails = new List<ipg_gateconfigurationdetail>();
                if (wftGroupRef != null)
                {
                    currentNode.Group = _service.Retrieve(ipg_wftaskgroup.EntityLogicalName, wftGroupRef.Id, new ColumnSet(true))
                        .ToEntity<ipg_wftaskgroup>();
                }
                gatingTree.Add(currentNode);
            }
            return currentNode;
        }        
    }
}
