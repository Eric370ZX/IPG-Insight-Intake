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
	public enum ipg_unsupportedprocedureState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Represents procedures not supported by the Carrier.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_unsupportedprocedure")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_unsupportedprocedure : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_CarrierId = "ipg_carrierid";
			public const string ipg_Description = "ipg_description";
			public const string ipg_EffectiveDate = "ipg_effectivedate";
			public const string ipg_ExpirationDate = "ipg_expirationdate";
			public const string ipg_externalid = "ipg_externalid";
			public const string ipg_name = "ipg_name";
			public const string ipg_ProcedureId = "ipg_procedureid";
			public const string ipg_unsupportedprocedureId = "ipg_unsupportedprocedureid";
			public const string Id = "ipg_unsupportedprocedureid";
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
			public const string business_unit_ipg_unsupportedprocedure = "business_unit_ipg_unsupportedprocedure";
			public const string ipg_account_ipg_unsupportedprocedure_CarrierName = "ipg_account_ipg_unsupportedprocedure_CarrierName";
			public const string ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId = "ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId";
			public const string lk_ipg_unsupportedprocedure_createdby = "lk_ipg_unsupportedprocedure_createdby";
			public const string lk_ipg_unsupportedprocedure_createdonbehalfby = "lk_ipg_unsupportedprocedure_createdonbehalfby";
			public const string lk_ipg_unsupportedprocedure_modifiedby = "lk_ipg_unsupportedprocedure_modifiedby";
			public const string lk_ipg_unsupportedprocedure_modifiedonbehalfby = "lk_ipg_unsupportedprocedure_modifiedonbehalfby";
			public const string team_ipg_unsupportedprocedure = "team_ipg_unsupportedprocedure";
			public const string user_ipg_unsupportedprocedure = "user_ipg_unsupportedprocedure";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_unsupportedprocedure() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_unsupportedprocedure";
		
		public const string EntitySchemaName = "ipg_unsupportedprocedure";
		
		public const string PrimaryIdAttribute = "ipg_unsupportedprocedureid";
		
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
		/// The carrier on which this procedure is not supported
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
		/// Details of the reason for a procedure not supported by the carrier.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_description")]
		public string ipg_Description
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_description");
			}
			set
			{
				this.OnPropertyChanging("ipg_Description");
				this.SetAttributeValue("ipg_description", value);
				this.OnPropertyChanged("ipg_Description");
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
		/// Date from where this rule will no longer be applicable
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
		/// The procedure that is not supported.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_procedureid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ProcedureId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_procedureid");
			}
			set
			{
				this.OnPropertyChanging("ipg_ProcedureId");
				this.SetAttributeValue("ipg_procedureid", value);
				this.OnPropertyChanged("ipg_ProcedureId");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_unsupportedprocedureid")]
		public System.Nullable<System.Guid> ipg_unsupportedprocedureId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_unsupportedprocedureid");
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedprocedureId");
				this.SetAttributeValue("ipg_unsupportedprocedureid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_unsupportedprocedureId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_unsupportedprocedureid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_unsupportedprocedureId = value;
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
		/// Status of the Unsupported Procedure
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_unsupportedprocedureState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_unsupportedprocedureState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_unsupportedprocedureState), optionSet.Value)));
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
		/// Reason for the status of the Unsupported Procedure
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
		/// Reason for the status of the Unsupported Procedure
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_unsupportedprocedure_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_unsupportedprocedure_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_unsupportedprocedure_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedprocedure_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_unsupportedprocedure_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_unsupportedprocedure_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedprocedure_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_unsupportedprocedure_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_unsupportedprocedure_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_unsupportedprocedure_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedprocedure_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_unsupportedprocedure_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedprocedure_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedprocedure_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedprocedure_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_unsupportedprocedure_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_unsupportedprocedure_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedprocedure_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_unsupportedprocedure_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedprocedure_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedprocedure_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_unsupportedprocedure_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_unsupportedprocedure_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_unsupportedprocedure_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_unsupportedprocedure
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_unsupportedprocedure")]
		public Insight.Intake.BusinessUnit business_unit_ipg_unsupportedprocedure
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_unsupportedprocedure", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_unsupportedprocedure_CarrierName
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_unsupportedprocedure_CarrierName")]
		public Insight.Intake.Account ipg_account_ipg_unsupportedprocedure_CarrierName
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_unsupportedprocedure_CarrierName", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_unsupportedprocedure_CarrierName");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_unsupportedprocedure_CarrierName", null, value);
				this.OnPropertyChanged("ipg_account_ipg_unsupportedprocedure_CarrierName");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_procedureid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId")]
		public Insight.Intake.ipg_procedurename ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_procedurename>("ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId");
				this.SetRelatedEntity<Insight.Intake.ipg_procedurename>("ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId", null, value);
				this.OnPropertyChanged("ipg_ipg_procedurename_ipg_unsupportedprocedure_ProcedureId");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedprocedure_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedprocedure_createdby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedprocedure_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedprocedure_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedprocedure_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedprocedure_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedprocedure_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedprocedure_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_unsupportedprocedure_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedprocedure_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_unsupportedprocedure_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedprocedure_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedprocedure_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedprocedure_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedprocedure_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_unsupportedprocedure_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_unsupportedprocedure_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_unsupportedprocedure_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedprocedure_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_unsupportedprocedure_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_unsupportedprocedure_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_unsupportedprocedure_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_unsupportedprocedure
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_unsupportedprocedure")]
		public Insight.Intake.Team team_ipg_unsupportedprocedure
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_unsupportedprocedure", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_unsupportedprocedure
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_unsupportedprocedure")]
		public Insight.Intake.SystemUser user_ipg_unsupportedprocedure
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_unsupportedprocedure", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_unsupportedprocedure(object anonymousType) : 
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
                        Attributes["ipg_unsupportedprocedureid"] = base.Id;
                        break;
                    case "ipg_unsupportedprocedureid":
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