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
	public enum ipg_contactemailpreferenceState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Related to Contact entity and denotes for which facilities (ContactsAccount entities) the portal Contact (user) wishes to receive email digests, as well as the digest frequency and types.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_contactemailpreference")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_contactemailpreference : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_contactemailpreferenceId = "ipg_contactemailpreferenceid";
			public const string Id = "ipg_contactemailpreferenceid";
			public const string ipg_ContactId = "ipg_contactid";
			public const string ipg_emailalertfrequency = "ipg_emailalertfrequency";
			public const string ipg_emailalerttype = "ipg_emailalerttype";
			public const string ipg_name = "ipg_name";
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
			public const string business_unit_ipg_contactemailpreference = "business_unit_ipg_contactemailpreference";
			public const string ipg_contact_ipg_contactemailpreference = "ipg_contact_ipg_contactemailpreference";
			public const string lk_ipg_contactemailpreference_createdby = "lk_ipg_contactemailpreference_createdby";
			public const string lk_ipg_contactemailpreference_createdonbehalfby = "lk_ipg_contactemailpreference_createdonbehalfby";
			public const string lk_ipg_contactemailpreference_modifiedby = "lk_ipg_contactemailpreference_modifiedby";
			public const string lk_ipg_contactemailpreference_modifiedonbehalfby = "lk_ipg_contactemailpreference_modifiedonbehalfby";
			public const string team_ipg_contactemailpreference = "team_ipg_contactemailpreference";
			public const string user_ipg_contactemailpreference = "user_ipg_contactemailpreference";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_contactemailpreference() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_contactemailpreference";
		
		public const string EntitySchemaName = "ipg_contactemailpreference";
		
		public const string PrimaryIdAttribute = "ipg_contactemailpreferenceid";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactemailpreferenceid")]
		public System.Nullable<System.Guid> ipg_contactemailpreferenceId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_contactemailpreferenceid");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactemailpreferenceId");
				this.SetAttributeValue("ipg_contactemailpreferenceid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_contactemailpreferenceId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactemailpreferenceid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_contactemailpreferenceId = value;
			}
		}
		
		/// <summary>
		/// Denotes the contact to which the email preference record is related.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ContactId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_contactid");
			}
			set
			{
				this.OnPropertyChanging("ipg_ContactId");
				this.SetAttributeValue("ipg_contactid", value);
				this.OnPropertyChanged("ipg_ContactId");
			}
		}
		
		/// <summary>
		/// Denotes the frequency with which a given portal user (Contact) would like to receive email alerts.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_emailalertfrequency")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_emailalertfrequency
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_emailalertfrequency");
			}
			set
			{
				this.OnPropertyChanging("ipg_emailalertfrequency");
				this.SetAttributeValue("ipg_emailalertfrequency", value);
				this.OnPropertyChanged("ipg_emailalertfrequency");
			}
		}
		
		/// <summary>
		/// Denotes the frequency with which a given portal user (Contact) would like to receive email alerts.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_emailalertfrequency")]
		public virtual ipg_FrequencyofEmailAlerts? ipg_emailalertfrequencyEnum
		{
			get
			{
				return ((ipg_FrequencyofEmailAlerts?)(EntityOptionSetEnum.GetEnum(this, "ipg_emailalertfrequency")));
			}
			set
			{
				this.OnPropertyChanging("ipg_emailalertfrequency");
				this.SetAttributeValue("ipg_emailalertfrequency", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_emailalertfrequency");
			}
		}
		
		/// <summary>
		/// Denotes the type of email alerts the user would like to receive.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_emailalerttype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_emailalerttype
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_emailalerttype");
			}
			set
			{
				this.OnPropertyChanging("ipg_emailalerttype");
				this.SetAttributeValue("ipg_emailalerttype", value);
				this.OnPropertyChanged("ipg_emailalerttype");
			}
		}
		
		/// <summary>
		/// Denotes the type of email alerts the user would like to receive.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_emailalerttype")]
		public virtual ipg_EmailAlertType? ipg_emailalerttypeEnum
		{
			get
			{
				return ((ipg_EmailAlertType?)(EntityOptionSetEnum.GetEnum(this, "ipg_emailalerttype")));
			}
			set
			{
				this.OnPropertyChanging("ipg_emailalerttype");
				this.SetAttributeValue("ipg_emailalerttype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_emailalerttype");
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
		/// Status of the Contact Email Preference
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_contactemailpreferenceState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_contactemailpreferenceState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_contactemailpreferenceState), optionSet.Value)));
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
		/// Reason for the status of the Contact Email Preference
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
		/// Reason for the status of the Contact Email Preference
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_contactemailpreference_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_contactemailpreference_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_contactemailpreference_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactemailpreference_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_contactemailpreference_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_contactemailpreference_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactemailpreference_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_contactemailpreference_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_contactemailpreference_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactemailpreference_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactemailpreference_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_contactemailpreference_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactemailpreference_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactemailpreference_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactemailpreference_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_contactemailpreference_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactemailpreference_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactemailpreference_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_contactemailpreference_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactemailpreference_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactemailpreference_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactemailpreference_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_contactemailpreference_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactemailpreference_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactemailpreference_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_contactemailpreference_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_contactemailpreference_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactemailpreference_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_contactemailpreference_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_contactemailpreference_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_contactemailpreference
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_contactemailpreference")]
		public Insight.Intake.BusinessUnit business_unit_ipg_contactemailpreference
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_contactemailpreference", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_contact_ipg_contactemailpreference
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contact_ipg_contactemailpreference")]
		public Insight.Intake.Contact ipg_contact_ipg_contactemailpreference
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Contact>("ipg_contact_ipg_contactemailpreference", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contact_ipg_contactemailpreference");
				this.SetRelatedEntity<Insight.Intake.Contact>("ipg_contact_ipg_contactemailpreference", null, value);
				this.OnPropertyChanged("ipg_contact_ipg_contactemailpreference");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactemailpreference_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactemailpreference_createdby")]
		public Insight.Intake.SystemUser lk_ipg_contactemailpreference_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactemailpreference_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactemailpreference_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactemailpreference_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_contactemailpreference_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactemailpreference_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_contactemailpreference_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactemailpreference_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_contactemailpreference_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactemailpreference_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactemailpreference_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_contactemailpreference_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactemailpreference_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactemailpreference_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactemailpreference_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_contactemailpreference_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactemailpreference_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_contactemailpreference_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactemailpreference_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_contactemailpreference_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_contactemailpreference
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_contactemailpreference")]
		public Insight.Intake.Team team_ipg_contactemailpreference
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_contactemailpreference", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_contactemailpreference
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_contactemailpreference")]
		public Insight.Intake.SystemUser user_ipg_contactemailpreference
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_contactemailpreference", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_contactemailpreference(object anonymousType) : 
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
                        Attributes["ipg_contactemailpreferenceid"] = base.Id;
                        break;
                    case "ipg_contactemailpreferenceid":
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