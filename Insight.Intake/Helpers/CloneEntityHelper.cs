using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Helpers
{
    public class CloneEntityHelper
    {
        private readonly IOrganizationService _service;
        private readonly OrganizationServiceContext _crmContext;

        public CloneEntityHelper(IOrganizationService service)
        {
            _service = service;
            _crmContext = new OrganizationServiceContext(_service);
        }

        public EntityReference CloneDocument(Entity document)
        {
            var exceptedFields = new string[] { "ipg_documentid", "statecode" };
            var newDocument = new Entity(document.LogicalName);

            CopyAttributes(newDocument, document, exceptedFields);

            newDocument.Id = _service.Create(newDocument);

            CopyAttachments(newDocument, document);
            return newDocument.ToEntityReference();
        }

        private void CopyAttributes(Entity newEntity, Entity entity, string[] exceptedFields)
        {
            foreach (var attribute in entity.Attributes)
            {
                if (!exceptedFields.Contains(attribute.Key))
                {
                    newEntity[attribute.Key] = attribute.Value;
                }
            }
        }

        private void CopyAttachments(Entity newEntity, Entity entity)
        {
            var attachments = (from note in _crmContext.CreateQuery<Annotation>()
                               where note.ObjectId.Id == entity.Id
                               select note).ToList();

            foreach (var attachment in attachments)
            {
                var newAttachment = new Annotation();
                CopyAttributes(newAttachment, attachment, new[] { "annotationid", "statecode" });
                newAttachment.ObjectId = newEntity.ToEntityReference();

                _service.Create(newAttachment);
            }
        }
    }
}
