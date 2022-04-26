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
	public enum ipg_contactsaccountsState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_contactsaccounts")]
	public partial class ipg_contactsaccounts : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_accountid = "ipg_accountid";
			public const string ipg_communicationpreference = "ipg_communicationpreference";
			public const string ipg_contactid = "ipg_contactid";
			public const string ipg_contactname = "ipg_contactname";
			public const string ipg_contactrolecode = "ipg_contactrolecode";
			public const string ipg_contactsaccountsId = "ipg_contactsaccountsid";
			public const string Id = "ipg_contactsaccountsid";
			public const string ipg_daystopay = "ipg_daystopay";
			public const string ipg_email = "ipg_email";
			public const string ipg_fax = "ipg_fax";
			public const string ipg_IsPrimaryContact = "ipg_isprimarycontact";
			public const string ipg_mainphone = "ipg_mainphone";
			public const string ipg_name = "ipg_name";
			public const string ipg_notes = "ipg_notes";
			public const string ipg_otherphone = "ipg_otherphone";
			public const string ipg_PCFControlsMyHelloWorldComp = "ipg_pcfcontrolsmyhelloworldcomp";
			public const string ipg_PCFControlsMyHelloWorldComp_Timestamp = "ipg_pcfcontrolsmyhelloworldcomp_timestamp";
			public const string ipg_PCFControlsMyHelloWorldComp_URL = "ipg_pcfcontrolsmyhelloworldcomp_url";
			public const string ipg_PCFControlsMyHelloWorldCompId = "ipg_pcfcontrolsmyhelloworldcompid";
			public const string ipg_rboname = "ipg_rboname";
			public const string ipg_titleorrole = "ipg_titleorrole";
			public const string ipg_user_accounts = "ipg_user_accounts";
			public const string ipg_user_roles = "ipg_user_roles";
			public const string ipg_vendorid = "ipg_vendorid";
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
			public const string ipg_account_ipg_contactsaccounts_Account = "ipg_account_ipg_contactsaccounts_Account";
			public const string ipg_contact_ipg_contactsaccounts_contactid = "ipg_contact_ipg_contactsaccounts_contactid";
			public const string lk_ipg_contactsaccounts_createdby = "lk_ipg_contactsaccounts_createdby";
			public const string lk_ipg_contactsaccounts_createdonbehalfby = "lk_ipg_contactsaccounts_createdonbehalfby";
			public const string lk_ipg_contactsaccounts_modifiedby = "lk_ipg_contactsaccounts_modifiedby";
			public const string lk_ipg_contactsaccounts_modifiedonbehalfby = "lk_ipg_contactsaccounts_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_contactsaccounts() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_contactsaccounts";
		
		public const string EntitySchemaName = "ipg_contactsaccounts";
		
		public const string PrimaryIdAttribute = "ipg_contactsaccountsid";
		
		public const string PrimaryNameAttribute = "ipg_name";
		
		public const string EntityLogicalCollectionName = "ipg_contactsaccountses";
		
		public const string EntitySetName = "ipg_contactsaccountses";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_accountid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_accountid
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_accountid");
			}
			set
			{
				this.OnPropertyChanging("ipg_accountid");
				this.SetAttributeValue("ipg_accountid", value);
				this.OnPropertyChanged("ipg_accountid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_communicationpreference")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_communicationpreference
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_communicationpreference");
			}
			set
			{
				this.OnPropertyChanging("ipg_communicationpreference");
				this.SetAttributeValue("ipg_communicationpreference", value);
				this.OnPropertyChanged("ipg_communicationpreference");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_communicationpreference")]
		public virtual ipg_contactsaccounts_ipg_communicationpreference? ipg_communicationpreferenceEnum
		{
			get
			{
				return ((ipg_contactsaccounts_ipg_communicationpreference?)(EntityOptionSetEnum.GetEnum(this, "ipg_communicationpreference")));
			}
			set
			{
				this.OnPropertyChanging("ipg_communicationpreference");
				this.SetAttributeValue("ipg_communicationpreference", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_communicationpreference");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_contactid
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_contactid");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactid");
				this.SetAttributeValue("ipg_contactid", value);
				this.OnPropertyChanged("ipg_contactid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactname")]
		public string ipg_contactname
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_contactname");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactname");
				this.SetAttributeValue("ipg_contactname", value);
				this.OnPropertyChanged("ipg_contactname");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactrolecode")]
		public Microsoft.Xrm.Sdk.OptionSetValueCollection ipg_contactrolecode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValueCollection>("ipg_contactrolecode");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactrolecode");
				this.SetAttributeValue("ipg_contactrolecode", value);
				this.OnPropertyChanged("ipg_contactrolecode");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactrolecode")]
		public virtual System.Collections.Generic.IEnumerable<ipg_contactsaccounts_ipg_ContactRoleCode> ipg_contactrolecodeEnum
		{
			get
			{
				return EntityOptionSetEnum.GetMultiEnum<ipg_contactsaccounts_ipg_ContactRoleCode>(this, "ipg_contactrolecode");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactrolecode");
				this.SetAttributeValue("ipg_contactrolecode", EntityOptionSetEnum.GetMultiEnum(this, "ipg_contactrolecode", value));
				this.OnPropertyChanged("ipg_contactrolecode");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactsaccountsid")]
		public System.Nullable<System.Guid> ipg_contactsaccountsId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_contactsaccountsid");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsaccountsId");
				this.SetAttributeValue("ipg_contactsaccountsid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_contactsaccountsId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactsaccountsid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_contactsaccountsId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_daystopay")]
		public string ipg_daystopay
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_daystopay");
			}
			set
			{
				this.OnPropertyChanging("ipg_daystopay");
				this.SetAttributeValue("ipg_daystopay", value);
				this.OnPropertyChanged("ipg_daystopay");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_email")]
		public string ipg_email
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_email");
			}
			set
			{
				this.OnPropertyChanging("ipg_email");
				this.SetAttributeValue("ipg_email", value);
				this.OnPropertyChanged("ipg_email");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_fax")]
		public string ipg_fax
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_fax");
			}
			set
			{
				this.OnPropertyChanging("ipg_fax");
				this.SetAttributeValue("ipg_fax", value);
				this.OnPropertyChanged("ipg_fax");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_isprimarycontact")]
		public System.Nullable<bool> ipg_IsPrimaryContact
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ipg_isprimarycontact");
			}
			set
			{
				this.OnPropertyChanging("ipg_IsPrimaryContact");
				this.SetAttributeValue("ipg_isprimarycontact", value);
				this.OnPropertyChanged("ipg_IsPrimaryContact");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_mainphone")]
		public string ipg_mainphone
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_mainphone");
			}
			set
			{
				this.OnPropertyChanging("ipg_mainphone");
				this.SetAttributeValue("ipg_mainphone", value);
				this.OnPropertyChanged("ipg_mainphone");
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_notes")]
		public string ipg_notes
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_notes");
			}
			set
			{
				this.OnPropertyChanging("ipg_notes");
				this.SetAttributeValue("ipg_notes", value);
				this.OnPropertyChanged("ipg_notes");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_otherphone")]
		public string ipg_otherphone
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_otherphone");
			}
			set
			{
				this.OnPropertyChanging("ipg_otherphone");
				this.SetAttributeValue("ipg_otherphone", value);
				this.OnPropertyChanged("ipg_otherphone");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_pcfcontrolsmyhelloworldcomp")]
		public byte[] ipg_PCFControlsMyHelloWorldComp
		{
			get
			{
				return this.GetAttributeValue<byte[]>("ipg_pcfcontrolsmyhelloworldcomp");
			}
			set
			{
				this.OnPropertyChanging("ipg_PCFControlsMyHelloWorldComp");
				this.SetAttributeValue("ipg_pcfcontrolsmyhelloworldcomp", value);
				this.OnPropertyChanged("ipg_PCFControlsMyHelloWorldComp");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_pcfcontrolsmyhelloworldcomp_timestamp")]
		public System.Nullable<long> ipg_PCFControlsMyHelloWorldComp_Timestamp
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("ipg_pcfcontrolsmyhelloworldcomp_timestamp");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_pcfcontrolsmyhelloworldcomp_url")]
		public string ipg_PCFControlsMyHelloWorldComp_URL
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_pcfcontrolsmyhelloworldcomp_url");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_pcfcontrolsmyhelloworldcompid")]
		public System.Nullable<System.Guid> ipg_PCFControlsMyHelloWorldCompId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_pcfcontrolsmyhelloworldcompid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_rboname")]
		public string ipg_rboname
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_rboname");
			}
			set
			{
				this.OnPropertyChanging("ipg_rboname");
				this.SetAttributeValue("ipg_rboname", value);
				this.OnPropertyChanged("ipg_rboname");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_titleorrole")]
		public string ipg_titleorrole
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_titleorrole");
			}
			set
			{
				this.OnPropertyChanging("ipg_titleorrole");
				this.SetAttributeValue("ipg_titleorrole", value);
				this.OnPropertyChanged("ipg_titleorrole");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_user_accounts")]
		public string ipg_user_accounts
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_user_accounts");
			}
			set
			{
				this.OnPropertyChanging("ipg_user_accounts");
				this.SetAttributeValue("ipg_user_accounts", value);
				this.OnPropertyChanged("ipg_user_accounts");
			}
		}
		
		/// <summary>
		/// String representation of Contact Role(ipg_contactrolecode) user for portal purposes.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_user_roles")]
		public string ipg_user_roles
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_user_roles");
			}
			set
			{
				this.OnPropertyChanging("ipg_user_roles");
				this.SetAttributeValue("ipg_user_roles", value);
				this.OnPropertyChanged("ipg_user_roles");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_vendorid")]
		public string ipg_vendorid
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_vendorid");
			}
			set
			{
				this.OnPropertyChanging("ipg_vendorid");
				this.SetAttributeValue("ipg_vendorid", value);
				this.OnPropertyChanged("ipg_vendorid");
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
		/// Status of the Contacts Account
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_contactsaccountsState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_contactsaccountsState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_contactsaccountsState), optionSet.Value)));
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
		/// Reason for the status of the Contacts Account
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
		/// Reason for the status of the Contacts Account
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_contactsaccounts_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_contactsaccounts_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_contactsaccounts_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsaccounts_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_contactsaccounts_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_contactsaccounts_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsaccounts_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_contactsaccounts_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_contactsaccounts_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactsaccounts_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsaccounts_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_contactsaccounts_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsaccounts_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsaccounts_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsaccounts_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_contactsaccounts_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactsaccounts_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsaccounts_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_contactsaccounts_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsaccounts_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsaccounts_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsaccounts_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_contactsaccounts_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactsaccounts_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsaccounts_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_contactsaccounts_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_contactsaccounts_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsaccounts_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_contactsaccounts_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_contactsaccounts_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_contactsaccounts_Account
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_accountid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_contactsaccounts_Account")]
		public Insight.Intake.Account ipg_account_ipg_contactsaccounts_Account
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_contactsaccounts_Account", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_contactsaccounts_Account");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_contactsaccounts_Account", null, value);
				this.OnPropertyChanged("ipg_account_ipg_contactsaccounts_Account");
			}
		}
		
		/// <summary>
		/// N:1 ipg_contact_ipg_contactsaccounts_contactid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contact_ipg_contactsaccounts_contactid")]
		public Insight.Intake.Contact ipg_contact_ipg_contactsaccounts_contactid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Contact>("ipg_contact_ipg_contactsaccounts_contactid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contact_ipg_contactsaccounts_contactid");
				this.SetRelatedEntity<Insight.Intake.Contact>("ipg_contact_ipg_contactsaccounts_contactid", null, value);
				this.OnPropertyChanged("ipg_contact_ipg_contactsaccounts_contactid");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsaccounts_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsaccounts_createdby")]
		public Insight.Intake.SystemUser lk_ipg_contactsaccounts_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsaccounts_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsaccounts_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsaccounts_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_contactsaccounts_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsaccounts_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_contactsaccounts_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsaccounts_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_contactsaccounts_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsaccounts_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsaccounts_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_contactsaccounts_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsaccounts_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsaccounts_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsaccounts_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_contactsaccounts_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsaccounts_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_contactsaccounts_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsaccounts_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_contactsaccounts_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_contactsaccounts(object anonymousType) : 
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
                        Attributes["ipg_contactsaccountsid"] = base.Id;
                        break;
                    case "ipg_contactsaccountsid":
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