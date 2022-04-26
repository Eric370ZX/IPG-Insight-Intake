using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckMedicalRecords : GatingPluginBase
    {

        #region Fault Messages Block
        private const string case1FaultMessage = @" Please Note: IPG was informed this case will be reviewed for medical necessity based on medical policy. IPG will require medical records to include the Operative Report and the Letter of Medical Necessity or H&P with diagnostic results prior to submitting this case to the health plan. Generally an operative report alone does not contain all of the necessary information to prove medical necessity.

Please forward the medical records at your earliest convenience. If you have any questions, please contact your CIM, {0}. Thank you for your assistance.";

        private const string case2FaultMessage = @" Please Note: IPG was informed this case will be reviewed for medical necessity based on medical policy. IPG will require medical records to include the Operative Report and the Letter of Medical Necessity or H&P with diagnostic results prior to submitting this case to the health plan. Generally an operative report alone does not contain all of the necessary information to prove medical necessity.

Please forward the medical records at your earliest convenience. If you have any questions, please contact your CIM, {0}. Thank you for your assistance.";

        private const string case3FaultMessage = @" Please Note: IPG was informed this case will be reviewed for medical necessity based on medical policy. IPG will require medical records to include the Operative Report and the Letter of Medical Necessity or H&P with diagnostic results prior to submitting this case to the health plan. Generally an operative report alone does not contain all of the necessary information to prove medical necessity.

For a trial SCS or interstim, we will also require records to show proof of a psychological evaluation and any conservative treatment. For perm SCS or interstim procedures, we will require the records to prove a 50% or better improvement from the trial. Please forward the medical records at your earliest convenience. If you have any questions, please contact your CIM, {0}. Thank you for your assistance.";

        private const string case4FaultMessage = @" Please Note: IPG was informed this case will be reviewed for medical necessity based on medical policy. IPG will require medical records to include the Operative Report and the Letter of Medical Necessity or H&P with diagnostic results prior to submitting this case to the health plan. Generally an operative report alone does not contain all of the necessary information to prove medical necessity.

For a trial SCS or interstim, we will also require records to show proof of a psychological evaluation and any conservative treatment. For perm SCS or interstim procedures, we will require the records to prove a 50% or better improvement from the trial. Please forward the medical records at your earliest convenience. If you have any questions, please contact your CIM, {0}. Thank you for your assistance.";

        private const string case5FaultMessage = @" Please Note: IPG was informed this case will be reviewed for medical necessity based on medical policy. IPG will require medical records to include the Operative Report and the Letter of Medical Necessity or H&P with diagnostic results prior to submitting this case to the health plan. Generally an operative report alone does not contain all of the necessary information to prove medical necessity.";

        private const string case6FaultMessage = @" Please Note: IPG was informed this case will be reviewed for medical necessity based on medical policy. IPG will require medical records to include the Operative Report and the Letter of Medical Necessity or H&P with diagnostic results prior to submitting this case to the health plan. Generally an operative report alone does not contain all of the necessary information to prove medical necessity.";

        #endregion

        public CheckMedicalRecords() : base("ipg_IPGGatingCheckMedicalRecords")
        {
        }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var negativeMessageBegin = "No Med Records required";
            var succeededMessageBegin = "Criteria Met";

            if (targetRef.LogicalName != Incident.EntityLogicalName)
            {
                return new GatingResponse
                {
                    Succeeded = false,
                    CaseNote = negativeMessageBegin,
                    PortalNote = negativeMessageBegin,
                };
            }
            var caseEntity = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_CarrierId,
                Incident.Fields.ipg_MemberIdNumber,
                Incident.Fields.ipg_CPTCodeId1,
                Incident.Fields.ipg_FacilityId)).ToEntity<Incident>();
            var facilityName = caseEntity.ipg_FacilityId?.Name ?? "--";
            var results = new List<CheckElement>{
                new CheckElement{
                    FaultMessage = string.Format(case1FaultMessage, facilityName),
                    Result = Case1(caseEntity.ipg_CarrierId, caseEntity.ipg_MemberIdNumber),
                },
                new CheckElement{
                    FaultMessage = string.Format(case2FaultMessage, facilityName),
                    Result = Case2(caseEntity.ipg_MemberIdNumber)
                },
                new CheckElement{
                    FaultMessage = string.Format(case3FaultMessage, facilityName),
                    Result = Case3(caseEntity.ipg_CarrierId, caseEntity.ipg_CPTCodeId1, targetRef),
                },
                new CheckElement{
                    FaultMessage = string.Format(case4FaultMessage, facilityName),
                    Result = Case4(caseEntity.ipg_CarrierId, caseEntity.ipg_CPTCodeId1, targetRef),
                },
                new CheckElement{
                    FaultMessage = case5FaultMessage,
                    Result = Case5(caseEntity.ipg_CarrierId, targetRef),
                },
                new CheckElement{
                    FaultMessage = case6FaultMessage,
                    Result = Case6(caseEntity.ipg_CarrierId, caseEntity.ipg_CPTCodeId1),
                },
                new CheckElement{
                    FaultMessage = "Operative Report and the Letter of Medical Necessity or H&P with diagnostic results (Case #7)",
                    Result = Case7(caseEntity.ipg_CarrierId, caseEntity.ipg_CPTCodeId1),
                },
                new CheckElement{
                    FaultMessage = "Operative Report and the Letter of Medical Necessity or H&P with diagnostic results (Case #8)",
                    Result = Case8(caseEntity.ipg_CarrierId, targetRef),
                }
            };
            var succeeded = results.All(x => x.Result == false);
            var negativeMessage = string.Join("\n", results.Where(x => !x.Result).Select(x => x.FaultMessage).ToArray());
            var note = succeeded
                ? succeededMessageBegin
                : negativeMessageBegin + "\n" + negativeMessage;

            if (!succeeded)
            {
                crmService.Update(new Incident()
                {
                    Id = targetRef.Id,
                    ipg_portalheadermultiplelines = negativeMessage
                });
            }

            return new GatingResponse
            {
                Succeeded = succeeded,
                CaseNote = note,
                PortalNote = note,
                CustomMessage = note
            };
        }


        private bool Case1(EntityReference carrierId, string memberId)
        {
            if (carrierId == null)
            {
                return false;
            }
            var carrierName = "BCBS FEP";
            var memberIdFirstSymbol = 'R';
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                        new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName),
                    }
                }
            };
            return (crmService.RetrieveMultiple(query).Entities.Any()) && memberId.IndexOf(memberIdFirstSymbol) == 0;
        }

        private bool Case2(string memberId) => memberId.IndexOf("NSA") == 0;

        private bool Case3(EntityReference carrierId, EntityReference cptCodeId1, EntityReference caseId)
        {
            if (carrierId == null || cptCodeId1 == null)
            {
                return false;
            }
            var carrierName1 = "BC-Anthem-CA";
            var carrierName2 = "BC-Anthem-NV";
            var cptCode1 = "63650";
            var hcpcsCode1 = "L8680";
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.Or,
                    Filters = {
                        new FilterExpression{
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName1),
                            }
                        },
                        new FilterExpression{
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName2),
                            }
                        },
                    }
                },
            };
            if (!crmService.RetrieveMultiple(query).Entities.Any())
            {
                return false;
            }
            query = new QueryExpression
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_cptcode.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcode1, ConditionOperator.Equal, cptCode1),
                        new ConditionExpression(ipg_cptcode.PrimaryIdAttribute, ConditionOperator.Equal, cptCodeId1.Id),
                    },
                }
            };
            var cptCodes = crmService.RetrieveMultiple(query);
            query = new QueryExpression
            {
                EntityName = ipg_casepartdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_casepartdetail.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, caseId.Id)
                    }
                },
                LinkEntities = {
                    new LinkEntity{
                        LinkFromEntityName = ipg_casepartdetail.EntityLogicalName,
                        LinkFromAttributeName = ipg_casepartdetail.Fields.ipg_hcpcscode,
                        LinkToEntityName = ipg_masterhcpcs.EntityLogicalName,
                        LinkToAttributeName = ipg_masterhcpcs.PrimaryIdAttribute,
                        LinkCriteria = {
                            Conditions = {
                                new ConditionExpression(ipg_masterhcpcs.Fields.ipg_name, ConditionOperator.Equal, hcpcsCode1),
                            }
                        }
                    }
                }
            };
            var hcpcsCodes = crmService.RetrieveMultiple(query);
            return (hcpcsCodes.Entities.Any() || cptCodes.Entities.Any());
        }

        private bool Case4(EntityReference carrierId, EntityReference cptCodeId1, EntityReference caseId)
        {
            if (carrierId == null || cptCodeId1 == null)
            {
                return false;
            }
            var carrierName1 = "BC-Anthem-CA";
            var carrierName2 = "BC-Anthem-NV";
            var cptCode1 = "63685";
            var hcpcsCode1 = "L8688";
            var hcpcsCode2 = "L8680";
            var hcpcsCode3 = "L8681";
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.Or,
                    Filters = {
                        new FilterExpression{
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName1),
                            }
                        },
                        new FilterExpression{
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName2),
                            }
                        },
                    }
                },
            };
            if (!crmService.RetrieveMultiple(query).Entities.Any())
            {
                return false;
            }
            query = new QueryExpression
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_cptcode.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcode1, ConditionOperator.Equal, cptCode1),
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcodeId, ConditionOperator.Equal, cptCodeId1.Id),
                    },
                }
            };
            var cptCodes = crmService.RetrieveMultiple(query);
            query = new QueryExpression
            {
                EntityName = ipg_casepartdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_casepartdetail.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, caseId.Id)
                    }
                },
                LinkEntities = {
                    new LinkEntity{
                        LinkFromEntityName = ipg_casepartdetail.EntityLogicalName,
                        LinkFromAttributeName = ipg_casepartdetail.Fields.ipg_hcpcscode,
                        LinkToEntityName = ipg_masterhcpcs.EntityLogicalName,
                        LinkToAttributeName = ipg_masterhcpcs.PrimaryIdAttribute,
                        LinkCriteria = {
                            Conditions = {
                                new ConditionExpression(ipg_masterhcpcs.Fields.ipg_name, ConditionOperator.Equal, hcpcsCode1),
                                new ConditionExpression(ipg_masterhcpcs.Fields.ipg_name, ConditionOperator.Equal, hcpcsCode2),
                                new ConditionExpression(ipg_masterhcpcs.Fields.ipg_name, ConditionOperator.Equal, hcpcsCode3),
                            },
                            FilterOperator = LogicalOperator.Or,
                        }
                    }
                }
            };
            var hcpcsCodes = crmService.RetrieveMultiple(query);
            return (hcpcsCodes.Entities.Any() || cptCodes.Entities.Any());
        }

        private bool Case5(EntityReference carrierId, EntityReference caseId)
        {
            if (carrierId == null)
            {
                return false;
            }
            var carrierName1 = "BCBSFL-CONTRACTED";
            var hcpcsCode1 = "L8699";
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName1),
                    }
                },
            };
            if (!crmService.RetrieveMultiple(query).Entities.Any())
            {
                return false;
            }
            query = new QueryExpression
            {
                EntityName = ipg_casepartdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_casepartdetail.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, caseId.Id)
                    }
                },
                LinkEntities = {
                    new LinkEntity{
                        LinkFromEntityName = ipg_casepartdetail.EntityLogicalName,
                        LinkFromAttributeName = ipg_casepartdetail.Fields.ipg_hcpcscode,
                        LinkToEntityName = ipg_masterhcpcs.EntityLogicalName,
                        LinkToAttributeName = ipg_masterhcpcs.PrimaryIdAttribute,
                        LinkCriteria = {
                            Conditions = {
                                new ConditionExpression(ipg_masterhcpcs.Fields.ipg_name, ConditionOperator.Equal, hcpcsCode1),
                            }
                        }
                    }
                }
            };
            var hcpcsCodes = crmService.RetrieveMultiple(query);
            return hcpcsCodes.Entities.Any();
        }

        private bool Case6(EntityReference carrierId, EntityReference cptCodeId1)
        {
            if (carrierId == null)
            {
                return false;
            }
            var carrierName1 = "BCBSNC-CONTRACTED";
            var cptCode1 = "63650";
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName1),
                    }
                },
            };
            if (!crmService.RetrieveMultiple(query).Entities.Any())
            {
                return false;
            }
            query = new QueryExpression
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_cptcode.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcode1, ConditionOperator.Equal, cptCode1),
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcodeId, ConditionOperator.Equal, cptCodeId1.Id),
                    },
                }
            };
            var cptCodes = crmService.RetrieveMultiple(query);
            return cptCodes.Entities.Any();

        }

        private bool Case7(EntityReference carrierId, EntityReference cptCodeId1)
        {
            if (carrierId == null)
            {
                return false;
            }
            var carrierName1 = "BCBSNC-CONTRACTED";
            var cptCode1 = "63685";
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName1),
                    }
                },
            };
            if (!crmService.RetrieveMultiple(query).Entities.Any())
            {
                return false;
            }
            query = new QueryExpression
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_cptcode.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcode1, ConditionOperator.Equal, cptCode1),
                        new ConditionExpression(ipg_cptcode.Fields.ipg_cptcodeId, ConditionOperator.Equal, cptCodeId1.Id),
                    },
                }
            };
            var cptCodes = crmService.RetrieveMultiple(query);
            return cptCodes.Entities.Any();

        }

        private bool Case8(EntityReference carrierId, EntityReference caseId)
        {
            if (carrierId == null)
            {
                return false;
            }
            var carrierName1 = "BCBSNC-CONTRACTED";
            var hcpcsCode1 = "L8680";
            var hcpcsCode2 = "L8687";
            var hcpcsCode3 = "L8699";
            var hcpcsCode4 = "L8699";
            var hcpcsCode5 = "L8699";

            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                                new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.Equal, carrierId.Id),
                                new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, carrierName1),
                    }
                },
            };
            if (!crmService.RetrieveMultiple(query).Entities.Any())
            {
                return false;
            }
            query = new QueryExpression
            {
                EntityName = ipg_casepartdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_casepartdetail.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, caseId.Id)
                    }
                },
                LinkEntities = {
                    new LinkEntity{
                        LinkFromEntityName = ipg_casepartdetail.EntityLogicalName,
                        LinkFromAttributeName = ipg_casepartdetail.Fields.ipg_hcpcscode,
                        LinkToEntityName = ipg_masterhcpcs.EntityLogicalName,
                        LinkToAttributeName = ipg_masterhcpcs.PrimaryIdAttribute,
                        LinkCriteria = {
                            Conditions = {
                                new ConditionExpression(ipg_masterhcpcs.Fields.ipg_name, ConditionOperator.In, new string[] {hcpcsCode1, hcpcsCode2, hcpcsCode3, hcpcsCode4, hcpcsCode5}),
                            },
                            FilterOperator = LogicalOperator.Or,
                        }
                    }
                }
            };
            var hcpcsCodes = crmService.RetrieveMultiple(query);
            return hcpcsCodes.Entities.Any();
        }

    }
    class CheckElement
    {
        public string FaultMessage;
        public bool Result;
    }
}
