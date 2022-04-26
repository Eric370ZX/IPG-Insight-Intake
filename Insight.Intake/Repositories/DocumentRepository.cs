using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Repositories
{
    public class DocumentRepository
    {
        private IOrganizationService _crmService;

        public DocumentRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public IEnumerable<Entity> GetNotRejectedActiveTissueRequestForms(Guid caseId, ColumnSet columnSet)
        {
            QueryExpression query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = columnSet,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseId),
                        new ConditionExpression(ipg_document.Fields.StateCode, ConditionOperator.Equal, (int)ipg_documentState.Active),
                        new ConditionExpression(ipg_document.Fields.ipg_ReviewStatus, ConditionOperator.NotEqual, (int)ipg_document_ipg_ReviewStatus.Rejected)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documenttype.EntityLogicalName,
                        ipg_document.Fields.ipg_DocumentTypeId, ipg_documenttype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation, ConditionOperator.Equal, "TRF")
                            }
                        }
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }

        public IEnumerable<ipg_document> GetActivePSDocsExcept(EntityReference caseId, string except, ColumnSet columnSet = null)
        {
            QueryExpression query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = columnSet ?? new ColumnSet(false),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseId.Id),
                        new ConditionExpression(ipg_document.Fields.ipg_isactivepatientstatement, ConditionOperator.Equal, true)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documentcategorytype.EntityLogicalName,
                        ipg_document.Fields.ipg_documenttypecategoryid, ipg_documentcategorytype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documentcategorytype.PrimaryNameAttribute, ConditionOperator.Equal, DocumentCategioryNames.PatientStatement)
                            }
                        }
                    },
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documenttype.EntityLogicalName,
                        ipg_document.Fields.ipg_DocumentTypeId, ipg_documenttype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(ipg_documenttype.PrimaryNameAttribute),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documenttype.PrimaryNameAttribute, ConditionOperator.NotLike, $"%{except}%")
                            }
                        }
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities.Select(d =>d.ToEntity<ipg_document>());
        }

        public ipg_document GetLastActivePSDoc(EntityReference caseId, ColumnSet columnSet = null)
        {
            QueryExpression query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                TopCount = 1, 
                Orders = {new OrderExpression(ipg_document.Fields.CreatedOn, OrderType.Descending) },
                ColumnSet = columnSet ?? new ColumnSet(ipg_document.Fields.ipg_name),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseId.Id),
                        new ConditionExpression(ipg_document.Fields.ipg_isactivepatientstatement, ConditionOperator.Equal, true)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documentcategorytype.EntityLogicalName,
                        ipg_document.Fields.ipg_documenttypecategoryid, ipg_documentcategorytype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documentcategorytype.PrimaryNameAttribute, ConditionOperator.Equal, DocumentCategioryNames.PatientStatement)
                            }
                        }
                    }
                }
                
            };

            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntity<ipg_document>();
        }

        public ipg_document GetActivePSDoc(EntityReference caseId, string type, ColumnSet columnSet = null)
        {
            QueryExpression query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                TopCount = 1,
                Orders = { new OrderExpression(ipg_document.Fields.CreatedOn, OrderType.Descending) },
                ColumnSet = columnSet ?? new ColumnSet(ipg_document.Fields.ipg_name),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseId.Id),
                        new ConditionExpression(ipg_document.Fields.ipg_isactivepatientstatement, ConditionOperator.Equal, true),
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documentcategorytype.EntityLogicalName,
                        ipg_document.Fields.ipg_documenttypecategoryid, ipg_documentcategorytype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documentcategorytype.PrimaryNameAttribute, ConditionOperator.Equal, DocumentCategioryNames.PatientStatement)
                            }
                        }
                    },
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documenttype.EntityLogicalName,
                        ipg_document.Fields.ipg_DocumentTypeId, ipg_documenttype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(ipg_documenttype.PrimaryNameAttribute),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documenttype.PrimaryNameAttribute, ConditionOperator.Like, $"%{type}%")
                            }
                        }
                    }
                }

            };

            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<ipg_document>();
        }

        public List<ipg_document> GetActivePSDocs(EntityReference caseId, string type, ColumnSet columnSet = null)
        {
            QueryExpression query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                Orders = { new OrderExpression(ipg_document.Fields.CreatedOn, OrderType.Descending) },
                ColumnSet = columnSet ?? new ColumnSet(ipg_document.Fields.ipg_name),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseId.Id),
                        new ConditionExpression(ipg_document.Fields.ipg_isactivepatientstatement, ConditionOperator.Equal, true),
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documentcategorytype.EntityLogicalName,
                        ipg_document.Fields.ipg_documenttypecategoryid, ipg_documentcategorytype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documentcategorytype.PrimaryNameAttribute, ConditionOperator.Equal, DocumentCategioryNames.PatientStatement)
                            }
                        }
                    },
                    new LinkEntity(
                        ipg_document.EntityLogicalName, ipg_documenttype.EntityLogicalName,
                        ipg_document.Fields.ipg_DocumentTypeId, ipg_documenttype.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(ipg_documenttype.PrimaryNameAttribute),
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(ipg_documenttype.PrimaryNameAttribute, ConditionOperator.Like, $"%{type}%")
                            }
                        }
                    }
                }

            };

            return _crmService.RetrieveMultiple(query).Entities.Select(x => x.ToEntity<ipg_document>()).ToList();
        }

        public void DeactivatePSDocsExceptP1(EntityReference incidentReference)
        {
            var openPSDocs = GetActivePSDocsExcept(incidentReference, PSType.P1);
            foreach (var psDoc in openPSDocs)
            {
                _crmService.Update(new ipg_document() { Id = psDoc.Id, ipg_isactivepatientstatement = false });
            }
        }
    }
}
