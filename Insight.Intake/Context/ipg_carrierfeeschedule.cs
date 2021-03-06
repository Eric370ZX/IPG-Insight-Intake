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
	public enum ipg_carrierfeescheduleState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Used as N:N relationship between Carrier and Feeschedule entities
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_carrierfeeschedule")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_carrierfeeschedule : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_Active = "ipg_active";
			public const string ipg_Banding = "ipg_banding";
			public const string ipg_carrierfeescheduleId = "ipg_carrierfeescheduleid";
			public const string Id = "ipg_carrierfeescheduleid";
			public const string ipg_carrierid = "ipg_carrierid";
			public const string ipg_DefaultNonDTMTier = "ipg_defaultnondtmtier";
			public const string ipg_description = "ipg_description";
			public const string ipg_effectivedate = "ipg_effectivedate";
			public const string ipg_expiredate = "ipg_expiredate";
			public const string ipg_feescheduleid = "ipg_feescheduleid";
			public const string ipg_name = "ipg_name";
			public const string ipg_todaysdate = "ipg_todaysdate";
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
			public const string business_unit_ipg_carrierfeeschedule = "business_unit_ipg_carrierfeeschedule";
			public const string ipg_account_ipg_carrierfeeschedule_carrierid = "ipg_account_ipg_carrierfeeschedule_carrierid";
			public const string ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid = "ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid";
			public const string lk_ipg_carrierfeeschedule_createdby = "lk_ipg_carrierfeeschedule_createdby";
			public const string lk_ipg_carrierfeeschedule_createdonbehalfby = "lk_ipg_carrierfeeschedule_createdonbehalfby";
			public const string lk_ipg_carrierfeeschedule_modifiedby = "lk_ipg_carrierfeeschedule_modifiedby";
			public const string lk_ipg_carrierfeeschedule_modifiedonbehalfby = "lk_ipg_carrierfeeschedule_modifiedonbehalfby";
			public const string team_ipg_carrierfeeschedule = "team_ipg_carrierfeeschedule";
			public const string user_ipg_carrierfeeschedule = "user_ipg_carrierfeeschedule";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_carrierfeeschedule() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_carrierfeeschedule";
		
		public const string EntitySchemaName = "ipg_carrierfeeschedule";
		
		public const string PrimaryIdAttribute = "ipg_carrierfeescheduleid";
		
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
		/// Determine if Today's date is between Effective and Expiration date. Works for null values as well
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_active")]
		public System.Nullable<bool> ipg_Active
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ipg_active");
			}
		}
		
		/// <summary>
		/// The DTM/Non-DTM banding percentage of the Fee schedule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_banding")]
		public System.Nullable<double> ipg_Banding
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<double>>("ipg_banding");
			}
			set
			{
				this.OnPropertyChanging("ipg_Banding");
				this.SetAttributeValue("ipg_banding", value);
				this.OnPropertyChanged("ipg_Banding");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierfeescheduleid")]
		public System.Nullable<System.Guid> ipg_carrierfeescheduleId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_carrierfeescheduleid");
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierfeescheduleId");
				this.SetAttributeValue("ipg_carrierfeescheduleid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_carrierfeescheduleId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierfeescheduleid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_carrierfeescheduleId = value;
			}
		}
		
		/// <summary>
		/// Carrier Id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_carrierid
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_carrierid");
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierid");
				this.SetAttributeValue("ipg_carrierid", value);
				this.OnPropertyChanged("ipg_carrierid");
			}
		}
		
		/// <summary>
		/// The Tier of the Carrier Fee Schedule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_defaultnondtmtier")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_DefaultNonDTMTier
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_defaultnondtmtier");
			}
			set
			{
				this.OnPropertyChanging("ipg_DefaultNonDTMTier");
				this.SetAttributeValue("ipg_defaultnondtmtier", value);
				this.OnPropertyChanged("ipg_DefaultNonDTMTier");
			}
		}
		
		/// <summary>
		/// The Tier of the Carrier Fee Schedule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_defaultnondtmtier")]
		public virtual ipg_carrierfeeschedule_ipg_DefaultNonDTMTier? ipg_DefaultNonDTMTierEnum
		{
			get
			{
				return ((ipg_carrierfeeschedule_ipg_DefaultNonDTMTier?)(EntityOptionSetEnum.GetEnum(this, "ipg_defaultnondtmtier")));
			}
			set
			{
				this.OnPropertyChanging("ipg_DefaultNonDTMTier");
				this.SetAttributeValue("ipg_defaultnondtmtier", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_DefaultNonDTMTier");
			}
		}
		
		/// <summary>
		/// Description
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_description")]
		public string ipg_description
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_description");
			}
			set
			{
				this.OnPropertyChanging("ipg_description");
				this.SetAttributeValue("ipg_description", value);
				this.OnPropertyChanged("ipg_description");
			}
		}
		
		/// <summary>
		/// Effective Date
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_effectivedate")]
		public System.Nullable<System.DateTime> ipg_effectivedate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ipg_effectivedate");
			}
			set
			{
				this.OnPropertyChanging("ipg_effectivedate");
				this.SetAttributeValue("ipg_effectivedate", value);
				this.OnPropertyChanged("ipg_effectivedate");
			}
		}
		
		/// <summary>
		/// Expire Date
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_expiredate")]
		public System.Nullable<System.DateTime> ipg_expiredate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ipg_expiredate");
			}
			set
			{
				this.OnPropertyChanging("ipg_expiredate");
				this.SetAttributeValue("ipg_expiredate", value);
				this.OnPropertyChanged("ipg_expiredate");
			}
		}
		
		/// <summary>
		/// Fee Schedule Id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_feescheduleid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_feescheduleid
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_feescheduleid");
			}
			set
			{
				this.OnPropertyChanging("ipg_feescheduleid");
				this.SetAttributeValue("ipg_feescheduleid", value);
				this.OnPropertyChanged("ipg_feescheduleid");
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
		/// Today Date
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_todaysdate")]
		public System.Nullable<System.DateTime> ipg_todaysdate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ipg_todaysdate");
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
		/// Status of the Carrier Feeschedule Relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_carrierfeescheduleState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_carrierfeescheduleState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_carrierfeescheduleState), optionSet.Value)));
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
		/// Reason for the status of the Carrier Feeschedule Relationship
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
		/// Reason for the status of the Carrier Feeschedule Relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_carrierfeeschedule_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_carrierfeeschedule_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_carrierfeeschedule_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_carrierfeeschedule_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_carrierfeeschedule_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_carrierfeeschedule_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierfeeschedule_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_carrierfeeschedule_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_carrierfeeschedule_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_carrierfeeschedule_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_carrierfeeschedule_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_carrierfeeschedule_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_carrierfeeschedule_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierfeeschedule_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_carrierfeeschedule_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_carrierfeeschedule_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_carrierfeeschedule_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_carrierfeeschedule_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_carrierfeeschedule_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_carrierfeeschedule_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierfeeschedule_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_carrierfeeschedule_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_carrierfeeschedule_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_carrierfeeschedule_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_carrierfeeschedule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_carrierfeeschedule")]
		public Insight.Intake.BusinessUnit business_unit_ipg_carrierfeeschedule
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_carrierfeeschedule", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_carrierfeeschedule_carrierid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_carrierfeeschedule_carrierid")]
		public Insight.Intake.Account ipg_account_ipg_carrierfeeschedule_carrierid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_carrierfeeschedule_carrierid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_carrierfeeschedule_carrierid");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_carrierfeeschedule_carrierid", null, value);
				this.OnPropertyChanged("ipg_account_ipg_carrierfeeschedule_carrierid");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_feescheduleid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid")]
		public Insight.Intake.ipg_feeschedule ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_feeschedule>("ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid");
				this.SetRelatedEntity<Insight.Intake.ipg_feeschedule>("ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid", null, value);
				this.OnPropertyChanged("ipg_ipg_feeschedule_ipg_carrierfeeschedule_feescheduleid");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_carrierfeeschedule_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_carrierfeeschedule_createdby")]
		public Insight.Intake.SystemUser lk_ipg_carrierfeeschedule_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_carrierfeeschedule_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_carrierfeeschedule_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_carrierfeeschedule_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_carrierfeeschedule_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_carrierfeeschedule_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_carrierfeeschedule_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_carrierfeeschedule_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_carrierfeeschedule_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_carrierfeeschedule_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_carrierfeeschedule_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_carrierfeeschedule_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_carrierfeeschedule_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_carrierfeeschedule_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_carrierfeeschedule_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_carrierfeeschedule_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_carrierfeeschedule_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_carrierfeeschedule_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_carrierfeeschedule_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_carrierfeeschedule_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_carrierfeeschedule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_carrierfeeschedule")]
		public Insight.Intake.Team team_ipg_carrierfeeschedule
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_carrierfeeschedule", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_carrierfeeschedule
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_carrierfeeschedule")]
		public Insight.Intake.SystemUser user_ipg_carrierfeeschedule
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_carrierfeeschedule", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_carrierfeeschedule(object anonymousType) : 
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
                        Attributes["ipg_carrierfeescheduleid"] = base.Id;
                        break;
                    case "ipg_carrierfeescheduleid":
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