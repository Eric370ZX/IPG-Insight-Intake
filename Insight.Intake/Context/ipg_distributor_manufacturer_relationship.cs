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
	public enum ipg_distributor_manufacturer_relationshipState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// This entity functions as an N:N relationship between Distributor and Manufacturer. This relationship has been deprecated and has been left here in order to prevent possible system issues.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_distributor_manufacturer_relationship")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_distributor_manufacturer_relationship : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_distributor_manufacturer_relationshipId = "ipg_distributor_manufacturer_relationshipid";
			public const string Id = "ipg_distributor_manufacturer_relationshipid";
			public const string ipg_DistributorId = "ipg_distributorid";
			public const string ipg_ManufacturerId = "ipg_manufacturerid";
			public const string ipg_name = "ipg_name";
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
			public const string ipg_account_ipg_distributor_manufacturer_relationship_Distributor = "ipg_account_ipg_distributor_manufacturer_relationship_Distributor";
			public const string ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer = "ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer";
			public const string lk_ipg_distributor_manufacturer_relationship_createdby = "lk_ipg_distributor_manufacturer_relationship_createdby";
			public const string lk_ipg_distributor_manufacturer_relationship_createdonbehalfby = "lk_ipg_distributor_manufacturer_relationship_createdonbehalfby";
			public const string lk_ipg_distributor_manufacturer_relationship_modifiedby = "lk_ipg_distributor_manufacturer_relationship_modifiedby";
			public const string lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby = "lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_distributor_manufacturer_relationship() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_distributor_manufacturer_relationship";
		
		public const string EntitySchemaName = "ipg_distributor_manufacturer_relationship";
		
		public const string PrimaryIdAttribute = "ipg_distributor_manufacturer_relationshipid";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_distributor_manufacturer_relationshipid")]
		public System.Nullable<System.Guid> ipg_distributor_manufacturer_relationshipId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_distributor_manufacturer_relationshipid");
			}
			set
			{
				this.OnPropertyChanging("ipg_distributor_manufacturer_relationshipId");
				this.SetAttributeValue("ipg_distributor_manufacturer_relationshipid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_distributor_manufacturer_relationshipId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_distributor_manufacturer_relationshipid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_distributor_manufacturer_relationshipId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_distributorid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_DistributorId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_distributorid");
			}
			set
			{
				this.OnPropertyChanging("ipg_DistributorId");
				this.SetAttributeValue("ipg_distributorid", value);
				this.OnPropertyChanged("ipg_DistributorId");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_manufacturerid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ManufacturerId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_manufacturerid");
			}
			set
			{
				this.OnPropertyChanging("ipg_ManufacturerId");
				this.SetAttributeValue("ipg_manufacturerid", value);
				this.OnPropertyChanged("ipg_ManufacturerId");
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
		/// Status of the Distributor Manufacturer Relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_distributor_manufacturer_relationshipState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_distributor_manufacturer_relationshipState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_distributor_manufacturer_relationshipState), optionSet.Value)));
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
		/// Reason for the status of the Distributor Manufacturer Relationship
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
		/// Reason for the status of the Distributor Manufacturer Relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_distributor_manufacturer_relationship_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_distributor_manufacturer_relationship_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_distributor_manufacturer_relationship_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_distributor_manufacturer_relationship_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_distributor_manufacturer_relationship_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_distributor_manufacturer_relationship_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_distributor_manufacturer_relationship_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_distributor_manufacturer_relationship_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_distributor_manufacturer_relationship_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_distributor_manufacturer_relationship_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_distributor_manufacturer_relationship_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_distributor_manufacturer_relationship_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_distributor_manufacturer_relationship_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_distributor_manufacturer_relationship_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_distributor_manufacturer_relationship_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_distributor_manufacturer_relationship_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_distributor_manufacturer_relationship_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_distributor_manufacturer_relationship_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_distributor_manufacturer_relationship_Distributor
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_distributorid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_distributor_manufacturer_relationship_Distributor")]
		public Insight.Intake.Account ipg_account_ipg_distributor_manufacturer_relationship_Distributor
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_distributor_manufacturer_relationship_Distributor", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_distributor_manufacturer_relationship_Distributor");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_distributor_manufacturer_relationship_Distributor", null, value);
				this.OnPropertyChanged("ipg_account_ipg_distributor_manufacturer_relationship_Distributor");
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_manufacturerid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer")]
		public Insight.Intake.Account ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer", null, value);
				this.OnPropertyChanged("ipg_account_ipg_distributor_manufacturer_relationship_Manufacturer");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_distributor_manufacturer_relationship_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_distributor_manufacturer_relationship_createdby")]
		public Insight.Intake.SystemUser lk_ipg_distributor_manufacturer_relationship_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_distributor_manufacturer_relationship_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_distributor_manufacturer_relationship_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_distributor_manufacturer_relationship_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_distributor_manufacturer_relationship_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_distributor_manufacturer_relationship_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_distributor_manufacturer_relationship_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_distributor_manufacturer_relationship_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_distributor_manufacturer_relationship_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_distributor_manufacturer_relationship_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_distributor_manufacturer_relationship_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_distributor_manufacturer_relationship_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_distributor_manufacturer_relationship_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_distributor_manufacturer_relationship_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_distributor_manufacturer_relationship(object anonymousType) : 
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
                        Attributes["ipg_distributor_manufacturer_relationshipid"] = base.Id;
                        break;
                    case "ipg_distributor_manufacturer_relationshipid":
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