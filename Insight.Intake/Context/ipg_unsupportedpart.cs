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
	public enum ipg_unsupportedpartState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// 
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_unsupportedpart")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_unsupportedpart : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_CarrierId = "ipg_carrierid";
			public const string ipg_EffectiveDate = "ipg_effectivedate";
			public const string ipg_externalid = "ipg_externalid";
			public const string ipg_name = "ipg_name";
			public const string ipg_Notes = "ipg_notes";
			public const string ipg_ProductId = "ipg_productid";
			public const string ipg_unsupportedpartId = "ipg_unsupportedpartid";
			public const string Id = "ipg_unsupportedpartid";
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
			public const string business_unit_ipg_unsupportedpart = "business_unit_ipg_unsupportedpart";
			public const string ipg_account_ipg_unsupportedpart_Carrier = "ipg_account_ipg_unsupportedpart_Carrier";
			public const string ipg_product_ipg_unsupportedpart_ProductId = "ipg_product_ipg_unsupportedpart_ProductId";
			public const string lk_ipg_unsupportedpart_createdby = "lk_ipg_unsupportedpart_createdby";
			public const string lk_ipg_unsupportedpart_createdonbehalfby = "lk_ipg_unsupportedpart_createdonbehalfby";
			public const string lk_ipg_unsupportedpart_modifiedby = "lk_ipg_unsupportedpart_modifiedby";
			public const string lk_ipg_unsupportedpart_modifiedonbehalfby = "lk_ipg_unsupportedpart_modifiedonbehalfby";
			public const string team_ipg_unsupportedpart = "team_ipg_unsupportedpart";
			public const string user_ipg_unsupportedpart = "user_ipg_unsupportedpart";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_unsupportedpart() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_unsupportedpart";
		
		public const string EntitySchemaName = "ipg_unsupportedpart";
		
		public const string PrimaryIdAttribute = "ipg_unsupportedpartid";
		
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
		/// Carrier lookup
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
		/// Date from where this rule will be applicable
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
		/// 
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
		/// Notes for why a part is not accepted for this carrier
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_notes")]
		public string ipg_Notes
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_notes");
			}
			set
			{
				this.OnPropertyChanging("ipg_Notes");
				this.SetAttributeValue("ipg_notes", value);
				this.OnPropertyChanged("ipg_Notes");
			}
		}
		
		/// <summary>
		/// Product lookup
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_productid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ProductId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_productid");
			}
			set
			{
				this.OnPropertyChanging("ipg_ProductId");
				this.SetAttributeValue("ipg_productid", value);
				this.OnPropertyChanged("ipg_ProductId");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_unsupportedpartid")]
		public System.Nullable<System.Guid> ipg_unsupportedpartId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_unsupportedpartid");
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedpartId");
				this.SetAttributeValue("ipg_unsupportedpartid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_unsupportedpartId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_unsupportedpartid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_unsupportedpartId = value;
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
		/// Status of the Unsupported Part
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_unsupportedpartState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_unsupportedpartState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_unsupportedpartState), optionSet.Value)));
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
		/// Reason for the status of the Unsupported Part
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
		/// Reason for the status of the Unsupported Part
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_unsupportedpart_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_unsupportedpart_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_unsupportedpart_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedpart_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_unsupportedpart_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_unsupportedpart_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedpart_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_unsupportedpart_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_unsupportedpart_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_unsupportedpart_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedpart_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_unsupportedpart_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedpart_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedpart_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedpart_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_unsupportedpart_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_unsupportedpart_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedpart_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_unsupportedpart_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedpart_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedpart_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedpart_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_unsupportedpart_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_unsupportedpart_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedpart_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_unsupportedpart_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_unsupportedpart_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedpart_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_unsupportedpart_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_unsupportedpart_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_unsupportedpart
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_unsupportedpart")]
		public Insight.Intake.BusinessUnit business_unit_ipg_unsupportedpart
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_unsupportedpart", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_unsupportedpart_Carrier
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_unsupportedpart_Carrier")]
		public Insight.Intake.Account ipg_account_ipg_unsupportedpart_Carrier
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_unsupportedpart_Carrier", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_unsupportedpart_Carrier");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_unsupportedpart_Carrier", null, value);
				this.OnPropertyChanged("ipg_account_ipg_unsupportedpart_Carrier");
			}
		}
		
		/// <summary>
		/// N:1 ipg_product_ipg_unsupportedpart_ProductId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_productid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_product_ipg_unsupportedpart_ProductId")]
		public Insight.Intake.Product ipg_product_ipg_unsupportedpart_ProductId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Product>("ipg_product_ipg_unsupportedpart_ProductId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_product_ipg_unsupportedpart_ProductId");
				this.SetRelatedEntity<Insight.Intake.Product>("ipg_product_ipg_unsupportedpart_ProductId", null, value);
				this.OnPropertyChanged("ipg_product_ipg_unsupportedpart_ProductId");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedpart_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedpart_createdby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedpart_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedpart_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedpart_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedpart_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedpart_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedpart_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_unsupportedpart_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedpart_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_unsupportedpart_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedpart_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedpart_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedpart_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedpart_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedpart_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedpart_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedpart_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedpart_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_unsupportedpart_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedpart_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_unsupportedpart_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_unsupportedpart
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_unsupportedpart")]
		public Insight.Intake.Team team_ipg_unsupportedpart
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_unsupportedpart", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_unsupportedpart
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_unsupportedpart")]
		public Insight.Intake.SystemUser user_ipg_unsupportedpart
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_unsupportedpart", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_unsupportedpart(object anonymousType) : 
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
                        Attributes["ipg_unsupportedpartid"] = base.Id;
                        break;
                    case "ipg_unsupportedpartid":
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