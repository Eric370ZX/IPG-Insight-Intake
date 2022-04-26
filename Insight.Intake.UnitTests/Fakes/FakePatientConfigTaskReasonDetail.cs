using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakePatientConfigTaskReasonDetail
    {
        public static void Fake_ipg_ipg_taskreasondetails_ipg_statementgene_RelationShip(this XrmFakedContext self, ipg_statementgenerationeventconfiguration statementconfig, ipg_taskreasondetails taskReasonDetail)
        {
            if (self.GetRelationship(ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName) == null)
            {
                self.AddRelationship(ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName, new XrmFakedRelationship
                {
                    IntersectEntity = ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName,
                    Entity1LogicalName = ipg_taskreasondetails.EntityLogicalName,
                    Entity1Attribute = ipg_taskreasondetails.PrimaryIdAttribute,
                    Entity2LogicalName = ipg_statementgenerationeventconfiguration.EntityLogicalName,
                    Entity2Attribute = ipg_statementgenerationeventconfiguration.PrimaryIdAttribute
                });
            }

            var request = new AssociateRequest()
            {
                Target = taskReasonDetail.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    statementconfig.ToEntityReference()
                },
                Relationship = new Relationship(ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName)
            };

            self.GetOrganizationService().Execute(request);
        }
    }
}
