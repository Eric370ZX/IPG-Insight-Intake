//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Insight.Intake
{
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public enum ipg_casecloseconfigurationState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Entity stores possible case reasons for case closing.
	///Security priveledges: give everyone read access (org level), edit, add - admin only
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_casecloseconfiguration")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_casecloseconfiguration : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_casecloseconfigurationId = "ipg_casecloseconfigurationid";
			public const string Id = "ipg_casecloseconfigurationid";
			public const string ipg_caseclosurereason = "ipg_caseclosurereason";
			public const string ipg_caseclosuretype = "ipg_caseclosuretype";
			public const string ipg_casedisplaystatusid = "ipg_casedisplaystatusid";
			public const string ipg_casestate = "ipg_casestate";
			public const string ipg_closedby = "ipg_closedby";
			public const string ipg_facilitycommunication = "ipg_facilitycommunication";
			public const string ipg_name = "ipg_name";
			public const string ipg_providerstatus = "ipg_providerstatus";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string business_unit_ipg_casecloseconfiguration = "business_unit_ipg_casecloseconfiguration";
			public const string ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid = "ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid";
			public const string lk_ipg_casecloseconfiguration_createdby = "lk_ipg_casecloseconfiguration_createdby";
			public const string lk_ipg_casecloseconfiguration_createdonbehalfby = "lk_ipg_casecloseconfiguration_createdonbehalfby";
			public const string lk_ipg_casecloseconfiguration_modifiedby = "lk_ipg_casecloseconfiguration_modifiedby";
			public const string lk_ipg_casecloseconfiguration_modifiedonbehalfby = "lk_ipg_casecloseconfiguration_modifiedonbehalfby";
			public const string team_ipg_casecloseconfiguration = "team_ipg_casecloseconfiguration";
			public const string user_ipg_casecloseconfiguration = "user_ipg_casecloseconfiguration";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_casecloseconfiguration() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_casecloseconfiguration";
		
		public const string EntitySchemaName = "ipg_casecloseconfiguration";
		
		public const string PrimaryIdAttribute = "ipg_casecloseconfigurationid";
		
		public const string PrimaryNameAttribute = "ipg_name";
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		private void OnPropertyChanged(string propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void OnPropertyChanging(string propertyName)
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}
		
		/// <summary>
		/// Date and time when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
			set
			{
				this.OnPropertyChanging("CreatedOnBehalfBy");
				this.SetAttributeValue("createdonbehalfby", value);
				this.OnPropertyChanged("CreatedOnBehalfBy");
			}
		}
		
		/// <summary>
		/// Sequence number of the import that created this record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("importsequencenumber")]
		public System.Nullable<int> ImportSequenceNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
			}
			set
			{
				this.OnPropertyChanging("ImportSequenceNumber");
				this.SetAttributeValue("importsequencenumber", value);
				this.OnPropertyChanged("ImportSequenceNumber");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_casecloseconfigurationid")]
		public System.Nullable<System.Guid> ipg_casecloseconfigurationId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_casecloseconfigurationid");
			}
			set
			{
				this.OnPropertyChanging("ipg_casecloseconfigurationId");
				this.SetAttributeValue("ipg_casecloseconfigurationid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_casecloseconfigurationId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_casecloseconfigurationid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_casecloseconfigurationId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_caseclosurereason")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_caseclosurereason
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_caseclosurereason");
			}
			set
			{
				this.OnPropertyChanging("ipg_caseclosurereason");
				this.SetAttributeValue("ipg_caseclosurereason", value);
				this.OnPropertyChanged("ipg_caseclosurereason");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_caseclosurereason")]
		public virtual ipg_Caseclosurereason? ipg_caseclosurereasonEnum
		{
			get
			{
				return ((ipg_Caseclosurereason?)(EntityOptionSetEnum.GetEnum(this, "ipg_caseclosurereason")));
			}
			set
			{
				this.OnPropertyChanging("ipg_caseclosurereason");
				this.SetAttributeValue("ipg_caseclosurereason", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_caseclosurereason");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_caseclosuretype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_caseclosuretype
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_caseclosuretype");
			}
			set
			{
				this.OnPropertyChanging("ipg_caseclosuretype");
				this.SetAttributeValue("ipg_caseclosuretype", value);
				this.OnPropertyChanged("ipg_caseclosuretype");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_caseclosuretype")]
		public virtual ipg_Caseclosuretype? ipg_caseclosuretypeEnum
		{
			get
			{
				return ((ipg_Caseclosuretype?)(EntityOptionSetEnum.GetEnum(this, "ipg_caseclosuretype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_caseclosuretype");
				this.SetAttributeValue("ipg_caseclosuretype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_caseclosuretype");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_casedisplaystatusid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_casedisplaystatusid
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_casedisplaystatusid");
			}
			set
			{
				this.OnPropertyChanging("ipg_casedisplaystatusid");
				this.SetAttributeValue("ipg_casedisplaystatusid", value);
				this.OnPropertyChanged("ipg_casedisplaystatusid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_casestate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_casestate
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_casestate");
			}
			set
			{
				this.OnPropertyChanging("ipg_casestate");
				this.SetAttributeValue("ipg_casestate", value);
				this.OnPropertyChanged("ipg_casestate");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_casestate")]
		public virtual ipg_CaseStateCodes? ipg_casestateEnum
		{
			get
			{
				return ((ipg_CaseStateCodes?)(EntityOptionSetEnum.GetEnum(this, "ipg_casestate")));
			}
			set
			{
				this.OnPropertyChanging("ipg_casestate");
				this.SetAttributeValue("ipg_casestate", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_casestate");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_closedby")]
		public string ipg_closedby
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_closedby");
			}
			set
			{
				this.OnPropertyChanging("ipg_closedby");
				this.SetAttributeValue("ipg_closedby", value);
				this.OnPropertyChanged("ipg_closedby");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_facilitycommunication")]
		public string ipg_facilitycommunication
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_facilitycommunication");
			}
			set
			{
				this.OnPropertyChanging("ipg_facilitycommunication");
				this.SetAttributeValue("ipg_facilitycommunication", value);
				this.OnPropertyChanged("ipg_facilitycommunication");
			}
		}
		
		/// <summary>
		/// Name of the rule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_name")]
		public string ipg_name
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_name");
			}
			set
			{
				this.OnPropertyChanging("ipg_name");
				this.SetAttributeValue("ipg_name", value);
				this.OnPropertyChanged("ipg_name");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_providerstatus")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_providerstatus
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_providerstatus");
			}
			set
			{
				this.OnPropertyChanging("ipg_providerstatus");
				this.SetAttributeValue("ipg_providerstatus", value);
				this.OnPropertyChanged("ipg_providerstatus");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_providerstatus")]
		public virtual ipg_ProviderStatus? ipg_providerstatusEnum
		{
			get
			{
				return ((ipg_ProviderStatus?)(EntityOptionSetEnum.GetEnum(this, "ipg_providerstatus")));
			}
			set
			{
				this.OnPropertyChanging("ipg_providerstatus");
				this.SetAttributeValue("ipg_providerstatus", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_providerstatus");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}
		
		/// <summary>
		/// Date and time when the record was modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
			set
			{
				this.OnPropertyChanging("ModifiedOnBehalfBy");
				this.SetAttributeValue("modifiedonbehalfby", value);
				this.OnPropertyChanged("ModifiedOnBehalfBy");
			}
		}
		
		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overriddencreatedon")]
		public System.Nullable<System.DateTime> OverriddenCreatedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
			}
			set
			{
				this.OnPropertyChanging("OverriddenCreatedOn");
				this.SetAttributeValue("overriddencreatedon", value);
				this.OnPropertyChanged("OverriddenCreatedOn");
			}
		}
		
		/// <summary>
		/// Owner Id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ownerid")]
		public Microsoft.Xrm.Sdk.EntityReference OwnerId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ownerid");
			}
			set
			{
				this.OnPropertyChanging("OwnerId");
				this.SetAttributeValue("ownerid", value);
				this.OnPropertyChanged("OwnerId");
			}
		}
		
		/// <summary>
		/// Unique identifier for the business unit that owns the record
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		public Microsoft.Xrm.Sdk.EntityReference OwningBusinessUnit
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningbusinessunit");
			}
		}
		
		/// <summary>
		/// Unique identifier for the team that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		public Microsoft.Xrm.Sdk.EntityReference OwningTeam
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningteam");
			}
		}
		
		/// <summary>
		/// Unique identifier for the user that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		public Microsoft.Xrm.Sdk.EntityReference OwningUser
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owninguser");
			}
		}
		
		/// <summary>
		/// Status of the Case close configuration
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_casecloseconfigurationState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_casecloseconfigurationState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_casecloseconfigurationState), optionSet.Value)));
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.OnPropertyChanging("StateCode");
				if ((value == null))
				{
					this.SetAttributeValue("statecode", null);
				}
				else
				{
					this.SetAttributeValue("statecode", new Microsoft.Xrm.Sdk.OptionSetValue(((int)(value))));
				}
				this.OnPropertyChanged("StateCode");
			}
		}
		
		/// <summary>
		/// Reason for the status of the Case close configuration
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public Microsoft.Xrm.Sdk.OptionSetValue StatusCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statuscode");
			}
			set
			{
				this.OnPropertyChanging("StatusCode");
				this.SetAttributeValue("statuscode", value);
				this.OnPropertyChanged("StatusCode");
			}
		}
		
		/// <summary>
		/// Reason for the status of the Case close configuration
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_casecloseconfiguration_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_casecloseconfiguration_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
			}
			set
			{
				this.OnPropertyChanging("StatusCode");
				this.SetAttributeValue("statuscode", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("StatusCode");
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("timezoneruleversionnumber")]
		public System.Nullable<int> TimeZoneRuleVersionNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("timezoneruleversionnumber");
			}
			set
			{
				this.OnPropertyChanging("TimeZoneRuleVersionNumber");
				this.SetAttributeValue("timezoneruleversionnumber", value);
				this.OnPropertyChanged("TimeZoneRuleVersionNumber");
			}
		}
		
		/// <summary>
		/// Time zone code that was in use when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("utcconversiontimezonecode")]
		public System.Nullable<int> UTCConversionTimeZoneCode
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("utcconversiontimezonecode");
			}
			set
			{
				this.OnPropertyChanging("UTCConversionTimeZoneCode");
				this.SetAttributeValue("utcconversiontimezonecode", value);
				this.OnPropertyChanged("UTCConversionTimeZoneCode");
			}
		}
		
		/// <summary>
		/// Version Number
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
		
		/// <summary>
		/// 1:N ipg_casecloseconfiguration_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_casecloseconfiguration_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_casecloseconfiguration_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_casecloseconfiguration_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_casecloseconfiguration_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_casecloseconfiguration_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_casecloseconfiguration_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_casecloseconfiguration_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_casecloseconfiguration_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_casecloseconfiguration_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_casecloseconfiguration_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_casecloseconfiguration_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_casecloseconfiguration_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_casecloseconfiguration_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_casecloseconfiguration_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_casecloseconfiguration_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_casecloseconfiguration_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_casecloseconfiguration_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_casecloseconfiguration_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_casecloseconfiguration_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_casecloseconfiguration_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_casecloseconfiguration_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_casecloseconfiguration
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_casecloseconfiguration")]
		public Insight.Intake.BusinessUnit business_unit_ipg_casecloseconfiguration
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_casecloseconfiguration", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_casedisplaystatusid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid")]
		public Insight.Intake.ipg_casestatusdisplayed ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_casestatusdisplayed>("ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid");
				this.SetRelatedEntity<Insight.Intake.ipg_casestatusdisplayed>("ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid", null, value);
				this.OnPropertyChanged("ipg_ipg_casestatusdisplayed_ipg_casecloseconfiguration_casedisplaystatusid");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_casecloseconfiguration_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_casecloseconfiguration_createdby")]
		public Insight.Intake.SystemUser lk_ipg_casecloseconfiguration_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_casecloseconfiguration_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_casecloseconfiguration_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_casecloseconfiguration_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_casecloseconfiguration_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_casecloseconfiguration_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_casecloseconfiguration_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_casecloseconfiguration_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_casecloseconfiguration_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_casecloseconfiguration_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_casecloseconfiguration_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_casecloseconfiguration_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_casecloseconfiguration_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_casecloseconfiguration_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_casecloseconfiguration_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_casecloseconfiguration_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_casecloseconfiguration_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_casecloseconfiguration_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_casecloseconfiguration_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_casecloseconfiguration_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_casecloseconfiguration
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_casecloseconfiguration")]
		public Insight.Intake.Team team_ipg_casecloseconfiguration
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_casecloseconfiguration", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_casecloseconfiguration
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_casecloseconfiguration")]
		public Insight.Intake.SystemUser user_ipg_casecloseconfiguration
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_casecloseconfiguration", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_casecloseconfiguration(object anonymousType) : 
				this()
		{
            foreach (var p in anonymousType.GetType().GetProperties())
            {
                var value = p.GetValue(anonymousType, null);
                var name = p.Name.ToLower();
            
                if (name.EndsWith("enum") && value.GetType().BaseType == typeof(System.Enum))
                {
                    value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);
                    name = name.Remove(name.Length - "enum".Length);
                }
            
                switch (name)
                {
                    case "id":
                        base.Id = (System.Guid)value;
                        Attributes["ipg_casecloseconfigurationid"] = base.Id;
                        break;
                    case "ipg_casecloseconfigurationid":
                        var id = (System.Nullable<System.Guid>) value;
                        if(id == null){ continue; }
                        base.Id = id.Value;
                        Attributes[name] = base.Id;
                        break;
                    case "formattedvalues":
                        // Add Support for FormattedValues
                        FormattedValues.AddRange((Microsoft.Xrm.Sdk.FormattedValueCollection)value);
                        break;
                    default:
                        Attributes[name] = value;
                        break;
                }
            }
		}
	}
}