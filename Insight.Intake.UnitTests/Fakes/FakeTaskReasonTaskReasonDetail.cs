using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeTaskReasonTaskReasonDetail
    {
        public static void Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(this XrmFakedContext self, ipg_taskreason taskReason, ipg_taskreasondetails taskReasonDetail)
        {
            if (self.GetRelationship(ipg_ipg_taskreasondetails_ipg_taskreason.EntityLogicalName) == null)
            {
                self.AddRelationship(ipg_ipg_taskreasondetails_ipg_taskreason.EntityLogicalName, new XrmFakedRelationship
                {
                    IntersectEntity = ipg_ipg_taskreasondetails_ipg_taskreason.EntityLogicalName,
                    Entity1LogicalName = ipg_taskreasondetails.EntityLogicalName,
                    Entity1Attribute = ipg_taskreasondetails.PrimaryIdAttribute,
                    Entity2LogicalName = ipg_taskreason.EntityLogicalName,
                    Entity2Attribute = ipg_taskreason.PrimaryIdAttribute
                });
            }

            var request = new AssociateRequest()
            {
                Target = taskReasonDetail.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    taskReason.ToEntityReference()
                },
                Relationship = new Relationship(ipg_ipg_taskreasondetails_ipg_taskreason.EntityLogicalName)
            };

            self.GetOrganizationService().Execute(request);
        }
    }
}
