using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Models
{
    public class GeneratePDFFromDocTemplateRequest
    {
        public EntityReference Target { get; set; }
        public string TemplateName { get; set; }
    }
}
