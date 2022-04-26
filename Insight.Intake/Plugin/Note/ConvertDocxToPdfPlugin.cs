using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.ServiceModel;
using System.Xml;

namespace Insight.Intake.Plugin.Note
{
    public class ConvertDocxToPdfPlugin : PluginBase
    {
        #region Secure/Unsecure Configuration Setup
        private readonly string _secureConfig = null;
        private readonly string _unsecureConfig = null;

        private readonly string _templateFileNamses;
        private readonly string _actionName;

        public ConvertDocxToPdfPlugin(string unsecureConfig, string secureConfig) : base(typeof(ConvertDocxToPdfPlugin))
        {
            var unsecureConfigXmlDoc = new XmlDocument();
            unsecureConfigXmlDoc.LoadXml(unsecureConfig);
            _templateFileNamses = PluginConfiguration.GetConfigDataString(unsecureConfigXmlDoc, "POTemplateFileNames");
            _actionName = PluginConfiguration.GetConfigDataString(unsecureConfigXmlDoc, "GeneratePdfActionName");
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;

            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Annotation.EntityLogicalName, PreOperationHandler);
        }

        #endregion

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            //ValidatePluginRegistration(context);

            try
            {
                var note = ((Entity)context.InputParameters["Target"]).ToEntity<Annotation>();
                var templateFileNames = _templateFileNamses.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (templateFileNames.Count == 0 || !templateFileNames.Contains(note.FileName) || (note.IsDocument.HasValue && note.IsDocument.Value == false))
                {
                    return;
                }

                //set action parameters
                OrganizationRequest req = new OrganizationRequest(_actionName);
                req["AnnotationId"] = note.Id.ToString();
                req["FileName"] = note.FileName;
                req["FileContent"] = note.DocumentBody;
                req["ObjectId"] = note.ObjectId;
                req["ObjectTypeCode"] = note.ObjectTypeCode;

                note.FileName = string.Empty;
                note.DocumentBody = string.Empty;
                //note.IsDocument = false;

                OrganizationResponse response = service.Execute(req);
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
            catch (Exception exception)
            {
                localPluginContext.Trace($"{this.GetType().Name}: {exception}");
                throw;
            }
        }

        private void ValidatePluginRegistration(IPluginExecutionContext executionContext)
        {
            if (executionContext.Mode != 0)
            {
                throw new Exception("Plugin must be registered as Synchronous.");
            }
            if (executionContext.Stage != 20) //PreOperation
            {
                throw new Exception("Plugin must be registered for PreOperation stage.");
            }
            if (!executionContext.MessageName.ToLower().Equals("create"))
            {
                throw new Exception("Plugin must be registered for Create message.");
            }
            if (executionContext.PrimaryEntityName != "annotation")
            {
                throw new Exception("Plugin must be registered for Note('annotation') entity.");
            }
        }
    }
}
