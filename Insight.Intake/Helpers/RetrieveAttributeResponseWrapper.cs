using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Insight.Intake.Helpers
{
    /// <summary>
    /// Wrapper class for the Xrm.Sdk.Messages.RetrieveAttributeResponse class. Primarily used to support Moq injection during testing.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/xrm/2011/Contracts")]
    public class RetrieveAttributeResponseWrapper : OrganizationResponse
    {
        public RetrieveAttributeResponseWrapper(OrganizationResponse response)
        {
            try
            {
                AttributeMetadata = ((RetrieveAttributeResponseWrapper)response).AttributeMetadata;
            }
            catch
            {
                AttributeMetadata = ((RetrieveAttributeResponse)response).AttributeMetadata;
            }
        }

        public AttributeMetadata AttributeMetadata { get; set; }
    }
}
