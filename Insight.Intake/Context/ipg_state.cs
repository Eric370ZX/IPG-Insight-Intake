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
	public enum ipg_stateState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_state")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_state : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_abbreviation = "ipg_abbreviation";
			public const string ipg_ExtAccessoryTaxType = "ipg_extaccessorytaxtype";
			public const string ipg_ExtComponentTaxType = "ipg_extcomponenttaxtype";
			public const string ipg_externalid = "ipg_externalid";
			public const string ipg_KitTaxType = "ipg_kittaxtype";
			public const string ipg_MedicalSupplyTaxType = "ipg_medicalsupplytaxtype";
			public const string ipg_name = "ipg_name";
			public const string ipg_PermImplantTaxType = "ipg_permimplanttaxtype";
			public const string ipg_stateId = "ipg_stateid";
			public const string Id = "ipg_stateid";
			public const string ipg_SurgicalToolTaxType = "ipg_surgicaltooltaxtype";
			public const string ipg_TaxRate = "ipg_taxrate";
			public const string ipg_TempImplantTaxType = "ipg_tempimplanttaxtype";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OrganizationId = "organizationid";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string lk_ipg_state_createdby = "lk_ipg_state_createdby";
			public const string lk_ipg_state_createdonbehalfby = "lk_ipg_state_createdonbehalfby";
			public const string lk_ipg_state_modifiedby = "lk_ipg_state_modifiedby";
			public const string lk_ipg_state_modifiedonbehalfby = "lk_ipg_state_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_state() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_state";
		
		public const string EntitySchemaName = "ipg_state";
		
		public const string PrimaryIdAttribute = "ipg_stateid";
		
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
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_abbreviation")]
		public string ipg_abbreviation
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_abbreviation");
			}
			set
			{
				this.OnPropertyChanging("ipg_abbreviation");
				this.SetAttributeValue("ipg_abbreviation", value);
				this.OnPropertyChanged("ipg_abbreviation");
			}
		}
		
		/// <summary>
		/// External Accessory Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_extaccessorytaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_ExtAccessoryTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_extaccessorytaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_ExtAccessoryTaxType");
				this.SetAttributeValue("ipg_extaccessorytaxtype", value);
				this.OnPropertyChanged("ipg_ExtAccessoryTaxType");
			}
		}
		
		/// <summary>
		/// External Accessory Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_extaccessorytaxtype")]
		public virtual ipg_TaxClassType? ipg_ExtAccessoryTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_extaccessorytaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_ExtAccessoryTaxType");
				this.SetAttributeValue("ipg_extaccessorytaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_ExtAccessoryTaxType");
			}
		}
		
		/// <summary>
		/// External Component Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_extcomponenttaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_ExtComponentTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_extcomponenttaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_ExtComponentTaxType");
				this.SetAttributeValue("ipg_extcomponenttaxtype", value);
				this.OnPropertyChanged("ipg_ExtComponentTaxType");
			}
		}
		
		/// <summary>
		/// External Component Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_extcomponenttaxtype")]
		public virtual ipg_TaxClassType? ipg_ExtComponentTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_extcomponenttaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_ExtComponentTaxType");
				this.SetAttributeValue("ipg_extcomponenttaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_ExtComponentTaxType");
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
		/// Kit Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_kittaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_KitTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_kittaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_KitTaxType");
				this.SetAttributeValue("ipg_kittaxtype", value);
				this.OnPropertyChanged("ipg_KitTaxType");
			}
		}
		
		/// <summary>
		/// Kit Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_kittaxtype")]
		public virtual ipg_TaxClassType? ipg_KitTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_kittaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_KitTaxType");
				this.SetAttributeValue("ipg_kittaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_KitTaxType");
			}
		}
		
		/// <summary>
		/// Medical Supply Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_medicalsupplytaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_MedicalSupplyTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_medicalsupplytaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_MedicalSupplyTaxType");
				this.SetAttributeValue("ipg_medicalsupplytaxtype", value);
				this.OnPropertyChanged("ipg_MedicalSupplyTaxType");
			}
		}
		
		/// <summary>
		/// Medical Supply Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_medicalsupplytaxtype")]
		public virtual ipg_TaxClassType? ipg_MedicalSupplyTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_medicalsupplytaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_MedicalSupplyTaxType");
				this.SetAttributeValue("ipg_medicalsupplytaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_MedicalSupplyTaxType");
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
		/// Permanent Implant Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_permimplanttaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_PermImplantTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_permimplanttaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_PermImplantTaxType");
				this.SetAttributeValue("ipg_permimplanttaxtype", value);
				this.OnPropertyChanged("ipg_PermImplantTaxType");
			}
		}
		
		/// <summary>
		/// Permanent Implant Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_permimplanttaxtype")]
		public virtual ipg_TaxClassType? ipg_PermImplantTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_permimplanttaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_PermImplantTaxType");
				this.SetAttributeValue("ipg_permimplanttaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_PermImplantTaxType");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_stateid")]
		public System.Nullable<System.Guid> ipg_stateId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_stateid");
			}
			set
			{
				this.OnPropertyChanging("ipg_stateId");
				this.SetAttributeValue("ipg_stateid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_stateId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_stateid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_stateId = value;
			}
		}
		
		/// <summary>
		/// Surgical Tool Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_surgicaltooltaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_SurgicalToolTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_surgicaltooltaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_SurgicalToolTaxType");
				this.SetAttributeValue("ipg_surgicaltooltaxtype", value);
				this.OnPropertyChanged("ipg_SurgicalToolTaxType");
			}
		}
		
		/// <summary>
		/// Surgical Tool Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_surgicaltooltaxtype")]
		public virtual ipg_TaxClassType? ipg_SurgicalToolTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_surgicaltooltaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_SurgicalToolTaxType");
				this.SetAttributeValue("ipg_surgicaltooltaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_SurgicalToolTaxType");
			}
		}
		
		/// <summary>
		/// Tax Rate
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taxrate")]
		public System.Nullable<decimal> ipg_TaxRate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("ipg_taxrate");
			}
			set
			{
				this.OnPropertyChanging("ipg_TaxRate");
				this.SetAttributeValue("ipg_taxrate", value);
				this.OnPropertyChanged("ipg_TaxRate");
			}
		}
		
		/// <summary>
		/// Temp Implant Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_tempimplanttaxtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_TempImplantTaxType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_tempimplanttaxtype");
			}
			set
			{
				this.OnPropertyChanging("ipg_TempImplantTaxType");
				this.SetAttributeValue("ipg_tempimplanttaxtype", value);
				this.OnPropertyChanged("ipg_TempImplantTaxType");
			}
		}
		
		/// <summary>
		/// Temp Implant Tax Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_tempimplanttaxtype")]
		public virtual ipg_TaxClassType? ipg_TempImplantTaxTypeEnum
		{
			get
			{
				return ((ipg_TaxClassType?)(EntityOptionSetEnum.GetEnum(this, "ipg_tempimplanttaxtype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_TempImplantTaxType");
				this.SetAttributeValue("ipg_tempimplanttaxtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_TempImplantTaxType");
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
		/// Unique identifier for the organization
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationid")]
		public Microsoft.Xrm.Sdk.EntityReference OrganizationId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("organizationid");
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
		/// Status of the State
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_stateState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_stateState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_stateState), optionSet.Value)));
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
		/// Reason for the status of the State
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
		/// Reason for the status of the State
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_state_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_state_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_ipg_state_account_billingstateid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_account_billingstateid")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Account> ipg_ipg_state_account_billingstateid
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Account>("ipg_ipg_state_account_billingstateid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_account_billingstateid");
				this.SetRelatedEntities<Insight.Intake.Account>("ipg_ipg_state_account_billingstateid", null, value);
				this.OnPropertyChanged("ipg_ipg_state_account_billingstateid");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_account_StateId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_account_StateId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Account> ipg_ipg_state_account_StateId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Account>("ipg_ipg_state_account_StateId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_account_StateId");
				this.SetRelatedEntities<Insight.Intake.Account>("ipg_ipg_state_account_StateId", null, value);
				this.OnPropertyChanged("ipg_ipg_state_account_StateId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_contact_PhysicianNPIState
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_contact_PhysicianNPIState")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Contact> ipg_ipg_state_contact_PhysicianNPIState
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Contact>("ipg_ipg_state_contact_PhysicianNPIState", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_contact_PhysicianNPIState");
				this.SetRelatedEntities<Insight.Intake.Contact>("ipg_ipg_state_contact_PhysicianNPIState", null, value);
				this.OnPropertyChanged("ipg_ipg_state_contact_PhysicianNPIState");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_contact_state
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_contact_state")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Contact> ipg_ipg_state_contact_state
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Contact>("ipg_ipg_state_contact_state", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_contact_state");
				this.SetRelatedEntities<Insight.Intake.Contact>("ipg_ipg_state_contact_state", null, value);
				this.OnPropertyChanged("ipg_ipg_state_contact_state");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_incident_jurisdictionstate
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_incident_jurisdictionstate")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Incident> ipg_ipg_state_incident_jurisdictionstate
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Incident>("ipg_ipg_state_incident_jurisdictionstate", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_incident_jurisdictionstate");
				this.SetRelatedEntities<Insight.Intake.Incident>("ipg_ipg_state_incident_jurisdictionstate", null, value);
				this.OnPropertyChanged("ipg_ipg_state_incident_jurisdictionstate");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_benefitsverificationform
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_benefitsverificationform")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_benefitsverificationform> ipg_ipg_state_ipg_benefitsverificationform
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_benefitsverificationform>("ipg_ipg_state_ipg_benefitsverificationform", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_benefitsverificationform");
				this.SetRelatedEntities<Insight.Intake.ipg_benefitsverificationform>("ipg_ipg_state_ipg_benefitsverificationform", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_benefitsverificationform");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_benefitsverificationform_1
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_benefitsverificationform_1")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_benefitsverificationform> ipg_ipg_state_ipg_benefitsverificationform_1
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_benefitsverificationform>("ipg_ipg_state_ipg_benefitsverificationform_1", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_benefitsverificationform_1");
				this.SetRelatedEntities<Insight.Intake.ipg_benefitsverificationform>("ipg_ipg_state_ipg_benefitsverificationform_1", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_benefitsverificationform_1");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_carrierstaterelationship_StateId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_carrierstaterelationship_StateId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_CarrierStateRelationship> ipg_ipg_state_ipg_carrierstaterelationship_StateId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_CarrierStateRelationship>("ipg_ipg_state_ipg_carrierstaterelationship_StateId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_carrierstaterelationship_StateId");
				this.SetRelatedEntities<Insight.Intake.ipg_CarrierStateRelationship>("ipg_ipg_state_ipg_carrierstaterelationship_StateId", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_carrierstaterelationship_StateId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_staterule_StateId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_staterule_StateId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_staterule> ipg_ipg_state_ipg_staterule_StateId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_staterule>("ipg_ipg_state_ipg_staterule_StateId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_staterule_StateId");
				this.SetRelatedEntities<Insight.Intake.ipg_staterule>("ipg_ipg_state_ipg_staterule_StateId", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_staterule_StateId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_State
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_State")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_State
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_State", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_State");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_State", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_State");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_StateId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_StateId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_StateId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_StateId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_StateId");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_StateId", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_StateId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_stateid2
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_stateid2")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_stateid2
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid2", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_stateid2");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid2", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_stateid2");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_stateid3
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_stateid3")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_stateid3
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid3", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_stateid3");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid3", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_stateid3");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_stateid5
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_stateid5")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_stateid5
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid5", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_stateid5");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid5", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_stateid5");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_stateid6
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_stateid6")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_stateid6
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid6", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_stateid6");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid6", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_stateid6");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_stateid7
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_stateid7")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_stateid7
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid7", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_stateid7");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid7", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_stateid7");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_state_ipg_zipcode_stateid8
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_state_ipg_zipcode_stateid8")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_zipcode> ipg_ipg_state_ipg_zipcode_stateid8
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid8", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_state_ipg_zipcode_stateid8");
				this.SetRelatedEntities<Insight.Intake.ipg_zipcode>("ipg_ipg_state_ipg_zipcode_stateid8", null, value);
				this.OnPropertyChanged("ipg_ipg_state_ipg_zipcode_stateid8");
			}
		}
		
		/// <summary>
		/// 1:N ipg_state_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_state_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_state_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_state_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_state_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_state_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_state_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_state_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_state_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_state_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_state_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_state_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_state_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_state_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_state_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_state_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_state_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_state_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_state_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_state_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_state_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_state_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_state_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_state_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_state_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_state_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_state_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_state_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:N ipg_ipg_carriernetwork_ipg_state
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_carriernetwork_ipg_state")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_carriernetwork> ipg_ipg_carriernetwork_ipg_state
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_carriernetwork>("ipg_ipg_carriernetwork_ipg_state", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_carriernetwork_ipg_state");
				this.SetRelatedEntities<Insight.Intake.ipg_carriernetwork>("ipg_ipg_carriernetwork_ipg_state", null, value);
				this.OnPropertyChanged("ipg_ipg_carriernetwork_ipg_state");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_state_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_state_createdby")]
		public Insight.Intake.SystemUser lk_ipg_state_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_state_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_state_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_state_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_state_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_state_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_state_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_state_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_state_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_state_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_state_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_state_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_state_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_state_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_state_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_state_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_state_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_state_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_state_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_state_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_state(object anonymousType) : 
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
                        Attributes["ipg_stateid"] = base.Id;
                        break;
                    case "ipg_stateid":
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