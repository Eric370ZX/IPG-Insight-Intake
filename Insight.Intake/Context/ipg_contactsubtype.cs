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
	public enum ipg_contactsubtypeState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_contactsubtype")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_contactsubtype : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_contactsubtypeId = "ipg_contactsubtypeid";
			public const string Id = "ipg_contactsubtypeid";
			public const string ipg_name = "ipg_name";
			public const string ipg_ParentContactType = "ipg_parentcontacttype";
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
			public const string business_unit_ipg_contactsubtype = "business_unit_ipg_contactsubtype";
			public const string ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType = "ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType";
			public const string lk_ipg_contactsubtype_createdby = "lk_ipg_contactsubtype_createdby";
			public const string lk_ipg_contactsubtype_createdonbehalfby = "lk_ipg_contactsubtype_createdonbehalfby";
			public const string lk_ipg_contactsubtype_modifiedby = "lk_ipg_contactsubtype_modifiedby";
			public const string lk_ipg_contactsubtype_modifiedonbehalfby = "lk_ipg_contactsubtype_modifiedonbehalfby";
			public const string team_ipg_contactsubtype = "team_ipg_contactsubtype";
			public const string user_ipg_contactsubtype = "user_ipg_contactsubtype";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_contactsubtype() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_contactsubtype";
		
		public const string EntitySchemaName = "ipg_contactsubtype";
		
		public const string PrimaryIdAttribute = "ipg_contactsubtypeid";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactsubtypeid")]
		public System.Nullable<System.Guid> ipg_contactsubtypeId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_contactsubtypeid");
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsubtypeId");
				this.SetAttributeValue("ipg_contactsubtypeid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_contactsubtypeId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_contactsubtypeid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_contactsubtypeId = value;
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
		/// Represents parent contact type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_parentcontacttype")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ParentContactType
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_parentcontacttype");
			}
			set
			{
				this.OnPropertyChanging("ipg_ParentContactType");
				this.SetAttributeValue("ipg_parentcontacttype", value);
				this.OnPropertyChanged("ipg_ParentContactType");
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
		/// Status of the Contact Sub Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_contactsubtypeState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_contactsubtypeState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_contactsubtypeState), optionSet.Value)));
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
		/// Reason for the status of the Contact Sub Type
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
		/// Reason for the status of the Contact Sub Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_contactsubtype_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_contactsubtype_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_contactsubtype_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsubtype_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_contactsubtype_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_contactsubtype_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsubtype_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_contactsubtype_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_contactsubtype_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactsubtype_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsubtype_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_contactsubtype_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsubtype_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsubtype_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsubtype_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_contactsubtype_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactsubtype_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsubtype_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_contactsubtype_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsubtype_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsubtype_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_contactsubtype_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_contactsubtype_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_contactsubtype_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_contactsubtype_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_contactsubtype_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_contactsubtype_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_contactsubtype_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_contactsubtype_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_contactsubtype_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_contactsubtype_contact_ContactSubTypeId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_contactsubtype_contact_ContactSubTypeId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Contact> ipg_ipg_contactsubtype_contact_ContactSubTypeId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Contact>("ipg_ipg_contactsubtype_contact_ContactSubTypeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_contactsubtype_contact_ContactSubTypeId");
				this.SetRelatedEntities<Insight.Intake.Contact>("ipg_ipg_contactsubtype_contact_ContactSubTypeId", null, value);
				this.OnPropertyChanged("ipg_ipg_contactsubtype_contact_ContactSubTypeId");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_ipg_contactsubtype
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_ipg_contactsubtype")]
		public Insight.Intake.BusinessUnit business_unit_ipg_contactsubtype
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_ipg_contactsubtype", null);
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_parentcontacttype")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType")]
		public Insight.Intake.ipg_contacttype ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_contacttype>("ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType");
				this.SetRelatedEntity<Insight.Intake.ipg_contacttype>("ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType", null, value);
				this.OnPropertyChanged("ipg_ipg_contacttype_ipg_contactsubtype_ParentContactType");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsubtype_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsubtype_createdby")]
		public Insight.Intake.SystemUser lk_ipg_contactsubtype_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsubtype_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsubtype_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsubtype_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_contactsubtype_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsubtype_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_contactsubtype_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsubtype_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_contactsubtype_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsubtype_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsubtype_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_contactsubtype_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsubtype_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_contactsubtype_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_contactsubtype_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_contactsubtype_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsubtype_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_contactsubtype_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_contactsubtype_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_contactsubtype_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_ipg_contactsubtype
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_ipg_contactsubtype")]
		public Insight.Intake.Team team_ipg_contactsubtype
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_ipg_contactsubtype", null);
			}
		}
		
		/// <summary>
		/// N:1 user_ipg_contactsubtype
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_ipg_contactsubtype")]
		public Insight.Intake.SystemUser user_ipg_contactsubtype
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_ipg_contactsubtype", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_contactsubtype(object anonymousType) : 
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
                        Attributes["ipg_contactsubtypeid"] = base.Id;
                        break;
                    case "ipg_contactsubtypeid":
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