using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Insight.Intake
{
    public partial class Incident
    {
        private Account _ipg_CarrierId_Entity;
        private Account _ipg_SecondaryCarrierId_Entity;
        private Account _ipg_FacilityId_Entity;
        private ipg_cptcode _ipg_CPTCodeId1_Entity;
        private ipg_cptcode _ipg_CPTCodeId2_Entity;
        private ipg_cptcode _ipg_CPTCodeId3_Entity;
        private ipg_cptcode _ipg_CPTCodeId4_Entity;
        private ipg_cptcode _ipg_CPTCodeId5_Entity;
        private ipg_cptcode _ipg_CPTCodeId6_Entity;
        private ipg_dxcode _ipg_DxCodeId1_Entity;
        private ipg_dxcode _ipg_DxCodeId2_Entity;
        private ipg_dxcode _ipg_DxCodeId3_Entity;
        private ipg_dxcode _ipg_DxCodeId4_Entity;
        private ipg_dxcode _ipg_DxCodeId5_Entity;
        private ipg_dxcode _ipg_DxCodeId6_Entity;
        private ipg_procedurename _ipg_procedureid_Entity;
        private EntityMetadata _incident_Metadata;
        public IOrganizationService CrmService { get; set; }
        public Account ipg_CarrierId_Entity
        {
            get
            {
                if (_ipg_CarrierId_Entity == null && this.ipg_CarrierId != null)
                {
                    _ipg_CarrierId_Entity = CrmService.Retrieve(Account.EntityLogicalName, this.ipg_CarrierId.Id,
                    new ColumnSet(true))
                    .ToEntity<Account>();
                    _ipg_CarrierId_Entity.CrmService = this.CrmService;
                }
                return _ipg_CarrierId_Entity;
            }
        }
        public Account ipg_SecondaryCarrierId_Entity
        {
            get
            {
                if (_ipg_SecondaryCarrierId_Entity == null && this.ipg_SecondaryCarrierId != null)
                {
                    _ipg_SecondaryCarrierId_Entity = CrmService.Retrieve(Account.EntityLogicalName, this.ipg_SecondaryCarrierId.Id,
                    new ColumnSet(true))
                    .ToEntity<Account>();
                    _ipg_SecondaryCarrierId_Entity.CrmService = this.CrmService;
                }
                return _ipg_SecondaryCarrierId_Entity;
            }
        }
        public Account ipg_FacilityId_Entity
        {
            get
            {
                if (_ipg_FacilityId_Entity == null && this.ipg_FacilityId != null)
                {
                    _ipg_FacilityId_Entity = CrmService.Retrieve(Account.EntityLogicalName, this.ipg_FacilityId.Id,
                    new ColumnSet(true))
                    .ToEntity<Account>();
                    _ipg_FacilityId_Entity.CrmService = this.CrmService;
                }
                return _ipg_FacilityId_Entity;
            }
        }
        public ipg_cptcode ipg_CPTCodeId1_Entity
        {
            get
            {
                if (_ipg_CPTCodeId1_Entity == null && this.ipg_CPTCodeId1 != null)
                {
                    _ipg_CPTCodeId1_Entity = CrmService.Retrieve(ipg_cptcode.EntityLogicalName, this.ipg_CPTCodeId1.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_cptcode>();
                    //_ipg_CPTCodeId1_Entity.CrmService = this.CrmService;
                }
                return _ipg_CPTCodeId1_Entity;
            }
        }
        public ipg_cptcode ipg_CPTCodeId2_Entity
        {
            get
            {
                if (_ipg_CPTCodeId1_Entity == null && this.ipg_CPTCodeId2 != null)
                {
                    _ipg_CPTCodeId2_Entity = CrmService.Retrieve(ipg_cptcode.EntityLogicalName, this.ipg_CPTCodeId2.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_cptcode>();
                    //_ipg_CPTCodeId1_Entity.CrmService = this.CrmService;
                }
                return _ipg_CPTCodeId2_Entity;
            }
        }
        public ipg_cptcode ipg_CPTCodeId3_Entity
        {
            get
            {
                if (_ipg_CPTCodeId3_Entity == null && this.ipg_CPTCodeId3 != null)
                {
                    _ipg_CPTCodeId3_Entity = CrmService.Retrieve(ipg_cptcode.EntityLogicalName, this.ipg_CPTCodeId3.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_cptcode>();
                    //_ipg_CPTCodeId3_Entity.CrmService = this.CrmService;
                }
                return _ipg_CPTCodeId3_Entity;
            }
        }
        public ipg_cptcode ipg_CPTCodeId4_Entity
        {
            get
            {
                if (_ipg_CPTCodeId4_Entity == null && this.ipg_CPTCodeId4 != null)
                {
                    _ipg_CPTCodeId4_Entity = CrmService.Retrieve(ipg_cptcode.EntityLogicalName, this.ipg_CPTCodeId4.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_cptcode>();
                    //_ipg_CPTCodeId4_Entity.CrmService = this.CrmService;
                }
                return _ipg_CPTCodeId4_Entity;
            }
        }
        public ipg_cptcode ipg_CPTCodeId5_Entity
        {
            get
            {
                if (_ipg_CPTCodeId5_Entity == null && this.ipg_CPTCodeId5 != null)
                {
                    _ipg_CPTCodeId5_Entity = CrmService.Retrieve(ipg_cptcode.EntityLogicalName, this.ipg_CPTCodeId5.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_cptcode>();
                    //_ipg_CPTCodeId5_Entity.CrmService = this.CrmService;
                }
                return _ipg_CPTCodeId5_Entity;
            }
        }
        public ipg_cptcode ipg_CPTCodeId6_Entity
        {
            get
            {
                if (_ipg_CPTCodeId6_Entity == null && this.ipg_CPTCodeId6 != null)
                {
                    _ipg_CPTCodeId6_Entity = CrmService.Retrieve(ipg_cptcode.EntityLogicalName, this.ipg_CPTCodeId6.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_cptcode>();
                    //_ipg_CPTCodeId6_Entity.CrmService = this.CrmService;
                }
                return _ipg_CPTCodeId6_Entity;
            }
        }
        public ipg_dxcode ipg_DxCodeId1_Entity
        {
            get
            {
                if (_ipg_DxCodeId1_Entity == null && this.ipg_DxCodeId1 != null)
                {
                    _ipg_DxCodeId1_Entity = CrmService.Retrieve(ipg_dxcode.EntityLogicalName, this.ipg_DxCodeId1.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_dxcode>();
                    //_ipg_DxCodeId1_Entity.CrmService = this.CrmService;
                }
                return _ipg_DxCodeId1_Entity;
            }
        }
        public ipg_dxcode ipg_DxCodeId2_Entity
        {
            get
            {
                if (_ipg_DxCodeId1_Entity == null && this.ipg_DxCodeId2 != null)
                {
                    _ipg_DxCodeId2_Entity = CrmService.Retrieve(ipg_dxcode.EntityLogicalName, this.ipg_DxCodeId2.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_dxcode>();
                    //_ipg_DxCodeId1_Entity.CrmService = this.CrmService;
                }
                return _ipg_DxCodeId2_Entity;
            }
        }
        public ipg_dxcode ipg_DxCodeId3_Entity
        {
            get
            {
                if (_ipg_DxCodeId3_Entity == null && this.ipg_DxCodeId3 != null)
                {
                    _ipg_DxCodeId3_Entity = CrmService.Retrieve(ipg_dxcode.EntityLogicalName, this.ipg_DxCodeId3.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_dxcode>();
                    //_ipg_DxCodeId3_Entity.CrmService = this.CrmService;
                }
                return _ipg_DxCodeId3_Entity;
            }
        }
        public ipg_dxcode ipg_DxCodeId4_Entity
        {
            get
            {
                if (_ipg_DxCodeId4_Entity == null && this.ipg_DxCodeId4 != null)
                {
                    _ipg_DxCodeId4_Entity = CrmService.Retrieve(ipg_dxcode.EntityLogicalName, this.ipg_DxCodeId4.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_dxcode>();
                    //_ipg_DxCodeId4_Entity.CrmService = this.CrmService;
                }
                return _ipg_DxCodeId4_Entity;
            }
        }
        public ipg_dxcode ipg_DxCodeId5_Entity
        {
            get
            {
                if (_ipg_DxCodeId5_Entity == null && this.ipg_DxCodeId5 != null)
                {
                    _ipg_DxCodeId5_Entity = CrmService.Retrieve(ipg_dxcode.EntityLogicalName, this.ipg_DxCodeId5.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_dxcode>();
                    //_ipg_DxCodeId5_Entity.CrmService = this.CrmService;
                }
                return _ipg_DxCodeId5_Entity;
            }
        }
        public ipg_dxcode ipg_DxCodeId6_Entity
        {
            get
            {
                if (_ipg_DxCodeId6_Entity == null && this.ipg_DxCodeId6 != null)
                {
                    _ipg_DxCodeId6_Entity = CrmService.Retrieve(ipg_dxcode.EntityLogicalName, this.ipg_DxCodeId6.Id,
                    new ColumnSet(true))
                    .ToEntity<ipg_dxcode>();
                    //_ipg_DxCodeId6_Entity.CrmService = this.CrmService;
                }
                return _ipg_DxCodeId6_Entity;
            }
        }
        public ipg_procedurename ipg_procedureid_Entity
        {
            get
            {
                if (_ipg_procedureid_Entity == null && this.ipg_procedureid != null)
                {
                    _ipg_procedureid_Entity = CrmService.Retrieve(ipg_procedurename.EntityLogicalName, this.ipg_procedureid.Id,
                    new ColumnSet(true)).ToEntity<ipg_procedurename>();
                    //_ipg_DxCodeId6_Entity.CrmService = this.CrmService;
                }
                return _ipg_procedureid_Entity;
            }
        }
        public EntityMetadata Metadata
        {
            get
            {
                if (_incident_Metadata == null)
                {
                    _incident_Metadata = CrmService.GetEntityMetadata(EntityLogicalName);
                }
                return _incident_Metadata;
            }
        }
    }

    public partial class ipg_referral
    {
        private Account _ipg_CarrierId_Entity;
        private ipg_document _ipg_SourceDocumentId_Entity;
        public IOrganizationService CrmService { get; set; }
        public Account ipg_CarrierId_Entity
        {
            get
            {
                if (_ipg_CarrierId_Entity == null && this.ipg_CarrierId != null)
                {
                    _ipg_CarrierId_Entity = CrmService.Retrieve(Account.EntityLogicalName, this.ipg_CarrierId.Id,
                    new ColumnSet(true)).ToEntity<Account>();
                }
                return _ipg_CarrierId_Entity;
            }
        }
        public ipg_document ipg_SourceDocumentId_Entity
        {
            get
            {
                if (_ipg_SourceDocumentId_Entity == null && this.ipg_SourceDocumentId != null)
                {
                    _ipg_SourceDocumentId_Entity = CrmService.Retrieve(ipg_document.EntityLogicalName, this.ipg_SourceDocumentId.Id,
                    new ColumnSet(true)).ToEntity<ipg_document>();
                    _ipg_SourceDocumentId_Entity.CrmService = this.CrmService;
                }
                return _ipg_SourceDocumentId_Entity;
            }
        }
    }
    public partial class Account
    {
        private SystemUser _ipg_FacilityCimId_Entity;
        private ipg_carriernetwork _ipg_carriernetworkid_Entity;
        public IOrganizationService CrmService { get; set; }
        public SystemUser ipg_FacilityCimId_Entity
        {
            get
            {
                if (_ipg_FacilityCimId_Entity == null && this.ipg_FacilityCimId != null)
                {
                    _ipg_FacilityCimId_Entity = CrmService.Retrieve(SystemUser.EntityLogicalName, this.ipg_FacilityCimId.Id,
                    new ColumnSet(true)).ToEntity<SystemUser>();
                   // _ipg_FacilityCimId_Entity.CrmService = this.CrmService;
                }
                return _ipg_FacilityCimId_Entity;
            }
        }
        public ipg_carriernetwork ipg_carriernetworkid_Entity
        {
            get
            {
                if (_ipg_carriernetworkid_Entity == null && this.ipg_carriernetworkid != null)
                {
                    _ipg_carriernetworkid_Entity = CrmService.Retrieve(ipg_carriernetwork.EntityLogicalName, this.ipg_carriernetworkid.Id,
                    new ColumnSet(true)).ToEntity<ipg_carriernetwork>();
                    // _ipg_carriernetwork_Entity.CrmService = this.CrmService;
                }
                return _ipg_carriernetworkid_Entity;
            }
        }
    }
    public partial class ipg_document
    {
        private ipg_documenttype _ipg_DocumentTypeId_Entity;
        public IOrganizationService CrmService { get; set; }
        public ipg_documenttype ipg_DocumentTypeId_Entity
        {
            get
            {
                if (_ipg_DocumentTypeId_Entity == null && this.ipg_DocumentTypeId != null)
                {
                    _ipg_DocumentTypeId_Entity = CrmService.Retrieve(ipg_documenttype.EntityLogicalName, this.ipg_DocumentTypeId.Id,
                    new ColumnSet(true)).ToEntity<ipg_documenttype>();
                    //_ipg_DocumentTypeId_Entity.CrmService = this.CrmService;
                }
                return _ipg_DocumentTypeId_Entity;
            }
        }
    }
    //public partial class ipg_documenttype
    //{
    //    private SystemUser _ipg_FacilityCimId_Entity;
    //    public IOrganizationService CrmService { get; set; }
    //    public SystemUser ipg_FacilityCimId_Entity
    //    {
    //        get
    //        {
    //            if (_ipg_FacilityCimId_Entity == null && this.ipg_FacilityCimId != null)
    //            {
    //                _ipg_FacilityCimId_Entity = CrmService.Retrieve(SystemUser.EntityLogicalName, this.ipg_FacilityCimId.Id,
    //                new ColumnSet(true)).ToEntity<SystemUser>();
    //            }
    //            return _ipg_FacilityCimId_Entity;
    //        }
    //    }
    //}
}

namespace Insight.Intake.Plugin.GatingV2
{
    public class GatingContext
    {
        public GatingContext(Guid? caseId, Guid? referralId, IOrganizationService crmService)
        {
            this.caseId = caseId;
            this.referralId = referralId;
            this.crmService = crmService;
            _case = null;
            _referral = null;
        }

        private readonly Guid? caseId;
        private readonly Guid? referralId;
        private readonly IOrganizationService crmService;


        private Incident _case;
        private ipg_referral _referral;

        public Incident Case
        {
            get
            {
                if (_case == null && caseId != null)
                {
                    _case = crmService.Retrieve(Incident.EntityLogicalName, caseId.Value, new ColumnSet(true))
                        .ToEntity<Incident>();
                    _case.CrmService = crmService;
                }
                return _case;
            }
            private set { _case = value; }
        }
        public ipg_referral Referral
        {
            get
            {
                if (_referral == null && referralId != null)
                {
                    _referral = crmService.Retrieve(ipg_referral.EntityLogicalName, referralId.Value, new ColumnSet(true))
                        .ToEntity<ipg_referral>();
                }
                return _referral;
            }
            private set { _referral = value; }
        }
    }
}
