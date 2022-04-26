using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.IO;

namespace Insight.Intake.Plugin.Document
{
    public class ValidateFileExtension : PluginBase
    {
        public ValidateFileExtension() : base(typeof(ValidateFileExtension))
        {
            RegisterEvent(
                PipelineStages.PreValidation,
                MessageNames.Create,
                ipg_document.EntityLogicalName,
                OnCreate);

            RegisterEvent(
                PipelineStages.PreValidation,
                MessageNames.Update,
                ipg_document.EntityLogicalName,
                OnUpdate);
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            var target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_document>();
            CheckDocumentExtension(pluginContext.OrganizationService, target);
        }

        private void OnUpdate(LocalPluginContext pluginContext)
        {
            var target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_document>();
            var document = pluginContext.OrganizationService.Retrieve(ipg_document.EntityLogicalName, 
                                                                 target.Id, 
                                                                 new ColumnSet(ipg_document.Fields.ipg_DocumentTypeId,
                                                                 ipg_document.Fields.ipg_CaseId)).ToEntity<ipg_document>();

            document.ipg_CaseId = target.Contains(ipg_document.Fields.ipg_CaseId) ? target.ipg_CaseId : document.ipg_CaseId;
            document.ipg_CaseId = target.Contains(ipg_document.Fields.ipg_DocumentTypeId) ? target.ipg_DocumentTypeId : document.ipg_DocumentTypeId;

            CheckDocumentExtension(pluginContext.OrganizationService, document);
        }

        private void CheckDocumentExtension(IOrganizationService organizationService, ipg_document doc)
        {
            if (doc.ipg_DocumentTypeId != null)
            {
                var docTypeAbr = organizationService.Retrieve(ipg_documenttype.EntityLogicalName,
                                                           doc.ipg_DocumentTypeId.Id,
                                                           new ColumnSet(ipg_documenttype.Fields.Id,
                                                           ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation))
                                                           .ToEntity<ipg_documenttype>()?.ipg_DocumentTypeAbbreviation?.ToLower() ?? "";
                if (docTypeAbr.Contains("pst") || docTypeAbr.Contains("psb"))
                {
                    //Skip this validation for patient statement since it has to save file as pdf and txt.
                    return;
                }
            }
            if (!string.IsNullOrEmpty(doc.ipg_FileName))
            {
                if (doc.ipg_CaseId == null)
                {
                    if (Path.GetExtension(doc.ipg_FileName).ToLower() != ".pdf")
                    {
                        throw new InvalidPluginExecutionException("Only pdf documents can be attached to the document.");
                    }
                }
                else
                {
                    var caseEntity = organizationService.Retrieve(Incident.EntityLogicalName,
                                                                  doc.ipg_CaseId.Id,
                                                                  new ColumnSet(Incident.Fields.ipg_CaseStatus))
                                                                  .ToEntity<Incident>();
                    if (caseEntity?.ipg_CaseStatus?.Value != (int)ipg_CaseStatus.Closed)
                    {
                        if (Path.GetExtension(doc.ipg_FileName).ToLower() != ".pdf")
                        {
                            throw new InvalidPluginExecutionException("Only pdf documents can be attached to the Open Case.");
                        }
                    }
                }
            }
        }

    }
}
