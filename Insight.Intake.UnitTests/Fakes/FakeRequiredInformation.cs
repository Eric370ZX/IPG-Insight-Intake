using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeRequiredInformation
    {
        public static Faker<ipg_requiredinformation> Fake(this ipg_requiredinformation self)
        {
            return new Faker<ipg_requiredinformation>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_requiredinformation> WithIncidentReference(this Faker<ipg_requiredinformation> self, Incident incident)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (incident == null) throw new ArgumentNullException(nameof(incident));

            self.RuleFor(x => x.ipg_CaseId, x => new EntityReference
            {
                LogicalName = incident.LogicalName,
                Id = incident.Id,
            });
            
            return self;
        }

        public static Faker<ipg_requiredinformation> WithDocumentTypeReference(this Faker<ipg_requiredinformation> self, ipg_documenttype documentType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (documentType == null) throw new ArgumentNullException(nameof(documentType));

            self.RuleFor(x => x.ipg_DocumentTypeId, x => new EntityReference
            {
                LogicalName = documentType.LogicalName,
                Id = documentType.Id,
            });

            return self;
        }
    }
}