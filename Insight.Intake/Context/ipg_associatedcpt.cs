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
	public enum ipg_associatedcptState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Represents association between facility / carrier and the CPT code.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_associatedcpt")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_associatedcpt : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_associatedcptId = "ipg_associatedcptid";
			public const string Id = "ipg_associatedcptid";
			public const string ipg_CarrierId = "ipg_carrierid";
			public const string ipg_CPTCodeId = "ipg_cptcodeid";
			public const string ipg_EffectiveDate = "ipg_effectivedate";
			public const string ipg_ExpirationDate = "ipg_expirationdate";
			public const string ipg_externalid = "ipg_externalid";
			public const string ipg_name = "ipg_name";
			public const string ipg_PreauthRequired = "ipg_preauthrequired";
			public const string ipg_Source = "ipg_source";
			public const string ipg_Supported = "ipg_supported";
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
			public const string business_unit_ipg_associatedcpt = "business_unit_ipg_associatedcpt";
			public const string ipg_account_ipg_associatedcpt_CarrierId = "ipg_account_ipg_associatedcpt_CarrierId";
			public const string ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId = "ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId";
			public const string lk_ipg_associatedcpt_createdby = "lk_ipg_associatedcpt_createdby";
			public const string lk_ipg_associatedcpt_createdonbehalfby = "lk_ipg_associatedcpt_createdonbehalfby";
			public const string lk_ipg_associatedcpt_modifiedby = "lk_ipg_associatedcpt_modifiedby";
			public const string lk_ipg_associatedcpt_modifiedonbehalfby = "lk_ipg_associatedcpt_modifiedonbehalfby";
			public const string team_ipg_associatedcpt = "team_ipg_associatedcpt";
			public const string user_ipg_associatedcpt = "user_ipg_associatedcpt";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_associatedcpt() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_associatedcpt";
		
		public const string EntitySchemaName = "ipg_associatedcpt";
		
		public const string PrimaryIdAttribute = "ipg_associatedcptid";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_associatedcptid")]
		public System.Nullable<System.Guid> ipg_associatedcptId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_associatedcptid");
			}
			set
			{
				this.OnPropertyChanging("ipg_associatedcptId");
				this.SetAttributeValue("ipg_associatedcptid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_associatedcptId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_associatedcptid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_associatedcptId = value;
			}
		}
		
		/// <summary>
		/// Represents Carrier record id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_CarrierId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_carrierid");
			}
			set
			{
				this.OnPropertyChanging("ipg_CarrierId");
				this.SetAttributeValue("ipg_carrierid", value);
				this.OnPropertyChanged("ipg_CarrierId");
			}
		}
		
		/// <summary>
		/// Represents CPT code id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_cptcodeid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_CPTCodeId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_cptcodeid");
			}
			set
			{
				this.OnPropertyChanging("ipg_CPTCodeId");
				this.SetAttributeValue("ipg_cptcodeid", value);
				this.OnPropertyChanged("ipg_CPTCodeId");
			}
		}
		
		/// <summary>
		/// Represents effective date for Carrier CPT rule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_effectivedate")]
		public System.Nullable<System.DateTime> ipg_EffectiveDate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ipg_effectivedate");
			}
			set
			{
				this.OnPropertyChanging("ipg_EffectiveDate");
				this.SetAttributeValue("ipg_effectivedate", value);
				this.OnPropertyChanged("ipg_EffectiveDate");
			}
		}
		
		/// <summary>
		/// Represents when Carrier CPT rule is going to expire
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_expirationdate")]
		public System.Nullable<System.DateTime> ipg_ExpirationDate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ipg_expirationdate");
			}
			set
			{
				this.OnPropertyChanging("ipg_ExpirationDate");
				this.SetAttributeValue("ipg_expirationdate", value);
				this.OnPropertyChanged("ipg_ExpirationDate");
			}
		}
		
		/// <summary>
		/// Represents legacy id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_externalid")]
		public string ipg_externalid
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_externalid");
			}
			set
			{
				this.OnPropertyChanging("ipg_externalid");
				this.SetAttributeValue("ipg_externalid", value);
				this.OnPropertyChanged("ipg_externalid");
			}
		}
		
		/// <summary>
		/// The name of the custom entity.
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
		/// Represents whether pre-authorization required for carrier-cpt combination
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_preauthrequired")]
		public System.Nullable<bool> ipg_PreauthRequired
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ipg_preauthrequired");
			}
			set
			{
				this.OnPropertyChanging("ipg_PreauthRequired");
				this.SetAttributeValue("ipg_preauthrequired", value);
				this.OnPropertyChanged("ipg_PreauthRequired");
			}
		}
		
		/// <summary>
		/// Source of CPT rule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_source")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_Source
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_source");
			}
			set
			{
				this.OnPropertyChanging("ipg_Source");
				this.SetAttributeValue("ipg_source", value);
				this.OnPropertyChanged("ipg_Source");
			}
		}
		
		/// <summary>
		/// Source of CPT rule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_source")]
		public virtual ipg_cptinclusionlisttype? ipg_SourceEnum
		{
			get
			{
				return ((ipg_cptinclusionlisttype?)(EntityOptionSetEnum.GetEnum(this, "ipg_source")));
			}
			set
			{
				this.OnPropertyChanging("ipg_Source");
				this.SetAttributeValue("ipg_source", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_Source");
			}
		}
		
		/// <summary>
		/// Indicates that the Cpt is supported by the Carrier
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_supported")]
		public System.Nullable<bool> ipg_Supported
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ipg_supported");
			}
			set
			{
				this.OnPropertyChanging("ipg_Supported");
				this.SetAttributeValue("ipg_supported", value);
				this.OnPropertyChanged("ipg_Supported");
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
		/// Status of the Associated CPT
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_associatedcptState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_associatedcptState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_associatedcptState), optionSet.Value)));
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
		/// Reason for the status of the Associated CPT
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
		/// Reason for the status of the Associated CPT
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_associatedcpt_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_associatedcpt_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_associatedcpt_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_associatedcpt_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_associatedcpt_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_associatedcpt_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_associatedcpt_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_associatedcpt_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_associatedcpt_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_associatedcpt_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_associatedcpt_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_associatedcpt_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_associatedcpt_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_associatedcpt_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_associatedcpt_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_associatedcpt_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_associatedcpt_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_associatedcpt_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_associatedcpt_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_associatedcpt_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_associatedcpt_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_associatedcpt_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_associatedcpt_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_associatedcpt_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_associatedcpt_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_associatedcpt_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_associatedcpt_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_associatedcpt_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_associatedcpt_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_associatedcpt_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_associatedcptdaterange> ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_associatedcptdaterange>("ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT");
				this.SetRelatedEntities<Insight.Intake.ipg_associatedcptdaterange>("ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT", null, value);
				this.OnPropertyChanged("ipg_ipg_associatedcpt_ipg_associatedcptdaterange_AssociatedCPT");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_associatedcpt
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_associatedcpt")]
		public Insight.Intake.BusinessUnit business_unit_ipg_associatedcpt
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_associatedcpt", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_associatedcpt_CarrierId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_associatedcpt_CarrierId")]
		public Insight.Intake.Account ipg_account_ipg_associatedcpt_CarrierId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_associatedcpt_CarrierId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_associatedcpt_CarrierId");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_associatedcpt_CarrierId", null, value);
				this.OnPropertyChanged("ipg_account_ipg_associatedcpt_CarrierId");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_cptcodeid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId")]
		public Insight.Intake.ipg_cptcode ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_cptcode>("ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId");
				this.SetRelatedEntity<Insight.Intake.ipg_cptcode>("ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId", null, value);
				this.OnPropertyChanged("ipg_ipg_cptcode_ipg_associatedcpt_CPTCodeId");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_associatedcpt_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_associatedcpt_createdby")]
		public Insight.Intake.SystemUser lk_ipg_associatedcpt_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_associatedcpt_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_associatedcpt_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_associatedcpt_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_associatedcpt_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_associatedcpt_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_associatedcpt_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_associatedcpt_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_associatedcpt_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_associatedcpt_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_associatedcpt_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_associatedcpt_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_associatedcpt_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_associatedcpt_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_associatedcpt_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_associatedcpt_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_associatedcpt_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_associatedcpt_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_associatedcpt_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_associatedcpt_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_associatedcpt
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_associatedcpt")]
		public Insight.Intake.Team team_ipg_associatedcpt
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_associatedcpt", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_associatedcpt
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_associatedcpt")]
		public Insight.Intake.SystemUser user_ipg_associatedcpt
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_associatedcpt", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_associatedcpt(object anonymousType) : 
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
                        Attributes["ipg_associatedcptid"] = base.Id;
                        break;
                    case "ipg_associatedcptid":
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