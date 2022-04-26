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
	public enum ipg_claimresponselineadjustmentState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_claimresponselineadjustment")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_claimresponselineadjustment : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ExchangeRate = "exchangerate";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_Amount = "ipg_amount";
			public const string ipg_Amount_new = "ipg_amount_new";
			public const string ipg_amount_new_Base = "ipg_amount_new_base";
			public const string ipg_claimresponselineadjustmentId = "ipg_claimresponselineadjustmentid";
			public const string Id = "ipg_claimresponselineadjustmentid";
			public const string ipg_ClaimResponseLineId = "ipg_claimresponselineid";
			public const string ipg_ClaimStatus = "ipg_claimstatus";
			public const string ipg_Code = "ipg_code";
			public const string ipg_name = "ipg_name";
			public const string ipg_Reason = "ipg_reason";
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
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string business_unit_ipg_claimresponselineadjustment = "business_unit_ipg_claimresponselineadjustment";
			public const string ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId = "ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId";
			public const string ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus = "ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus";
			public const string lk_ipg_claimresponselineadjustment_createdby = "lk_ipg_claimresponselineadjustment_createdby";
			public const string lk_ipg_claimresponselineadjustment_createdonbehalfby = "lk_ipg_claimresponselineadjustment_createdonbehalfby";
			public const string lk_ipg_claimresponselineadjustment_modifiedby = "lk_ipg_claimresponselineadjustment_modifiedby";
			public const string lk_ipg_claimresponselineadjustment_modifiedonbehalfby = "lk_ipg_claimresponselineadjustment_modifiedonbehalfby";
			public const string team_ipg_claimresponselineadjustment = "team_ipg_claimresponselineadjustment";
			public const string TransactionCurrency_ipg_claimresponselineadjustment = "TransactionCurrency_ipg_claimresponselineadjustment";
			public const string user_ipg_claimresponselineadjustment = "user_ipg_claimresponselineadjustment";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_claimresponselineadjustment() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_claimresponselineadjustment";
		
		public const string EntitySchemaName = "ipg_claimresponselineadjustment";
		
		public const string PrimaryIdAttribute = "ipg_claimresponselineadjustmentid";
		
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
		/// Exchange rate for the currency associated with the entity with respect to the base currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("exchangerate")]
		public System.Nullable<decimal> ExchangeRate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("exchangerate");
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
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_amount")]
		public System.Nullable<decimal> ipg_Amount
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("ipg_amount");
			}
			set
			{
				this.OnPropertyChanging("ipg_Amount");
				this.SetAttributeValue("ipg_amount", value);
				this.OnPropertyChanged("ipg_Amount");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_amount_new")]
		public Microsoft.Xrm.Sdk.Money ipg_Amount_new
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("ipg_amount_new");
			}
			set
			{
				this.OnPropertyChanging("ipg_Amount_new");
				this.SetAttributeValue("ipg_amount_new", value);
				this.OnPropertyChanged("ipg_Amount_new");
			}
		}
		
		/// <summary>
		/// Value of the Amount in base currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_amount_new_base")]
		public Microsoft.Xrm.Sdk.Money ipg_amount_new_Base
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("ipg_amount_new_base");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimresponselineadjustmentid")]
		public System.Nullable<System.Guid> ipg_claimresponselineadjustmentId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_claimresponselineadjustmentid");
			}
			set
			{
				this.OnPropertyChanging("ipg_claimresponselineadjustmentId");
				this.SetAttributeValue("ipg_claimresponselineadjustmentid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_claimresponselineadjustmentId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimresponselineadjustmentid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_claimresponselineadjustmentId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimresponselineid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ClaimResponseLineId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_claimresponselineid");
			}
			set
			{
				this.OnPropertyChanging("ipg_ClaimResponseLineId");
				this.SetAttributeValue("ipg_claimresponselineid", value);
				this.OnPropertyChanged("ipg_ClaimResponseLineId");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimstatus")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ClaimStatus
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_claimstatus");
			}
			set
			{
				this.OnPropertyChanging("ipg_ClaimStatus");
				this.SetAttributeValue("ipg_claimstatus", value);
				this.OnPropertyChanged("ipg_ClaimStatus");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_code")]
		public string ipg_Code
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_code");
			}
			set
			{
				this.OnPropertyChanging("ipg_Code");
				this.SetAttributeValue("ipg_code", value);
				this.OnPropertyChanged("ipg_Code");
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
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_reason")]
		public string ipg_Reason
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_reason");
			}
			set
			{
				this.OnPropertyChanging("ipg_Reason");
				this.SetAttributeValue("ipg_reason", value);
				this.OnPropertyChanged("ipg_Reason");
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
		/// Status of the Claim Response Line Adjustment
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_claimresponselineadjustmentState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_claimresponselineadjustmentState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_claimresponselineadjustmentState), optionSet.Value)));
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
		/// Reason for the status of the Claim Response Line Adjustment
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
		/// Reason for the status of the Claim Response Line Adjustment
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_claimresponselineadjustment_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_claimresponselineadjustment_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// Unique identifier of the currency associated with the entity.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		public Microsoft.Xrm.Sdk.EntityReference TransactionCurrencyId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("transactioncurrencyid");
			}
			set
			{
				this.OnPropertyChanging("TransactionCurrencyId");
				this.SetAttributeValue("transactioncurrencyid", value);
				this.OnPropertyChanged("TransactionCurrencyId");
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
		/// 1:N ipg_claimresponselineadjustment_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_claimresponselineadjustment_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_claimresponselineadjustment_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_claimresponselineadjustment_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_claimresponselineadjustment_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_claimresponselineadjustment_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_claimresponselineadjustment_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_claimresponselineadjustment_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_claimresponselineadjustment_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_claimresponselineadjustment_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_claimresponselineadjustment_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_claimresponselineadjustment_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_claimresponselineadjustment_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_claimresponselineadjustment_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_claimresponselineadjustment_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_claimresponselineadjustment_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_claimresponselineadjustment_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_claimresponselineadjustment_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_claimresponselineadjustment_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_claimresponselineadjustment_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_claimresponselineadjustment_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_claimresponselineadjustment_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_claimresponselineadjustment
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_claimresponselineadjustment")]
		public Insight.Intake.BusinessUnit business_unit_ipg_claimresponselineadjustment
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_claimresponselineadjustment", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimresponselineid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId")]
		public Insight.Intake.ipg_claimresponseline ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_claimresponseline>("ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId");
				this.SetRelatedEntity<Insight.Intake.ipg_claimresponseline>("ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId", null, value);
				this.OnPropertyChanged("ipg_ipg_claimresponseline_ipg_claimresponselineadjustment_ClaimResponseLineId");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimstatus")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus")]
		public Insight.Intake.ipg_claimstatuscode ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_claimstatuscode>("ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus");
				this.SetRelatedEntity<Insight.Intake.ipg_claimstatuscode>("ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus", null, value);
				this.OnPropertyChanged("ipg_ipg_claimstatuscode_ipg_claimresponselineadjustment_ClaimStatus");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_claimresponselineadjustment_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_claimresponselineadjustment_createdby")]
		public Insight.Intake.SystemUser lk_ipg_claimresponselineadjustment_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_claimresponselineadjustment_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_claimresponselineadjustment_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_claimresponselineadjustment_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_claimresponselineadjustment_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_claimresponselineadjustment_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_claimresponselineadjustment_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_claimresponselineadjustment_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_claimresponselineadjustment_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_claimresponselineadjustment_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_claimresponselineadjustment_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_claimresponselineadjustment_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_claimresponselineadjustment_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_claimresponselineadjustment_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_claimresponselineadjustment_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_claimresponselineadjustment_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_claimresponselineadjustment_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_claimresponselineadjustment_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_claimresponselineadjustment_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_claimresponselineadjustment_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_claimresponselineadjustment
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_claimresponselineadjustment")]
		public Insight.Intake.Team team_ipg_claimresponselineadjustment
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_claimresponselineadjustment", null);
			}
		}
		
		/// <summary>
		/// N:1 TransactionCurrency_ipg_claimresponselineadjustment
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("TransactionCurrency_ipg_claimresponselineadjustment")]
		public Insight.Intake.TransactionCurrency TransactionCurrency_ipg_claimresponselineadjustment
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.TransactionCurrency>("TransactionCurrency_ipg_claimresponselineadjustment", null);
			}
			set
			{
				this.OnPropertyChanging("TransactionCurrency_ipg_claimresponselineadjustment");
				this.SetRelatedEntity<Insight.Intake.TransactionCurrency>("TransactionCurrency_ipg_claimresponselineadjustment", null, value);
				this.OnPropertyChanged("TransactionCurrency_ipg_claimresponselineadjustment");
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_claimresponselineadjustment
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_claimresponselineadjustment")]
		public Insight.Intake.SystemUser user_ipg_claimresponselineadjustment
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_claimresponselineadjustment", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_claimresponselineadjustment(object anonymousType) : 
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
                        Attributes["ipg_claimresponselineadjustmentid"] = base.Id;
                        break;
                    case "ipg_claimresponselineadjustmentid":
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