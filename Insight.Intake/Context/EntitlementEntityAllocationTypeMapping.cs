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
	public enum EntitlementEntityAllocationTypeMappingState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Mapping to define which Allocation Types are available for Entity Type to be used on Entitlement
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("entitlemententityallocationtypemapping")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class EntitlementEntityAllocationTypeMapping : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string allocationtype = "allocationtype";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string EntitlementEntityAllocationTypeMappingId = "entitlemententityallocationtypemappingid";
			public const string Id = "entitlemententityallocationtypemappingid";
			public const string entitytype = "entitytype";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string name = "name";
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
			public const string business_unit_entitlemententityallocationtypemapping = "business_unit_entitlemententityallocationtypemapping";
			public const string lk_entitlemententityallocationtypemapping_createdby = "lk_entitlemententityallocationtypemapping_createdby";
			public const string lk_entitlemententityallocationtypemapping_createdonbehalfby = "lk_entitlemententityallocationtypemapping_createdonbehalfby";
			public const string lk_entitlemententityallocationtypemapping_modifiedby = "lk_entitlemententityallocationtypemapping_modifiedby";
			public const string lk_entitlemententityallocationtypemapping_modifiedonbehalfby = "lk_entitlemententityallocationtypemapping_modifiedonbehalfby";
			public const string team_entitlemententityallocationtypemapping = "team_entitlemententityallocationtypemapping";
			public const string user_entitlemententityallocationtypemapping = "user_entitlemententityallocationtypemapping";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public EntitlementEntityAllocationTypeMapping() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "entitlemententityallocationtypemapping";
		
		public const string EntitySchemaName = "EntitlementEntityAllocationTypeMapping";
		
		public const string PrimaryIdAttribute = "entitlemententityallocationtypemappingid";
		
		public const string PrimaryNameAttribute = "name";
		
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
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("allocationtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue allocationtype
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("allocationtype");
			}
			set
			{
				this.OnPropertyChanging("allocationtype");
				this.SetAttributeValue("allocationtype", value);
				this.OnPropertyChanged("allocationtype");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("allocationtype")]
		public virtual AllocationType? allocationtypeEnum
		{
			get
			{
				return ((AllocationType?)(EntityOptionSetEnum.GetEnum(this, "allocationtype")));
			}
			set
			{
				this.OnPropertyChanging("allocationtype");
				this.SetAttributeValue("allocationtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("allocationtype");
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
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitlemententityallocationtypemappingid")]
		public System.Nullable<System.Guid> EntitlementEntityAllocationTypeMappingId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("entitlemententityallocationtypemappingid");
			}
			set
			{
				this.OnPropertyChanging("EntitlementEntityAllocationTypeMappingId");
				this.SetAttributeValue("entitlemententityallocationtypemappingid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("EntitlementEntityAllocationTypeMappingId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitlemententityallocationtypemappingid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.EntitlementEntityAllocationTypeMappingId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitytype")]
		public Microsoft.Xrm.Sdk.OptionSetValue entitytype
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("entitytype");
			}
			set
			{
				this.OnPropertyChanging("entitytype");
				this.SetAttributeValue("entitytype", value);
				this.OnPropertyChanged("entitytype");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitytype")]
		public virtual EntityType? entitytypeEnum
		{
			get
			{
				return ((EntityType?)(EntityOptionSetEnum.GetEnum(this, "entitytype")));
			}
			set
			{
				this.OnPropertyChanging("entitytype");
				this.SetAttributeValue("entitytype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("entitytype");
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
		/// The name of the custom entity.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("name")]
		public string name
		{
			get
			{
				return this.GetAttributeValue<string>("name");
			}
			set
			{
				this.OnPropertyChanging("name");
				this.SetAttributeValue("name", value);
				this.OnPropertyChanged("name");
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
		/// Status of the Entitlement Entity Allocation Type Mapping
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.EntitlementEntityAllocationTypeMappingState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.EntitlementEntityAllocationTypeMappingState)(System.Enum.ToObject(typeof(Insight.Intake.EntitlementEntityAllocationTypeMappingState), optionSet.Value)));
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
		/// Reason for the status of the Entitlement Entity Allocation Type Mapping
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
		/// Reason for the status of the Entitlement Entity Allocation Type Mapping
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual EntitlementEntityAllocationTypeMapping_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((EntitlementEntityAllocationTypeMapping_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N entitlemententityallocationtypemapping_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("entitlemententityallocationtypemapping_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> entitlemententityallocationtypemapping_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("entitlemententityallocationtypemapping_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("entitlemententityallocationtypemapping_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("entitlemententityallocationtypemapping_AsyncOperations", null, value);
				this.OnPropertyChanged("entitlemententityallocationtypemapping_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N entitlemententityallocationtypemapping_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("entitlemententityallocationtypemapping_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> entitlemententityallocationtypemapping_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("entitlemententityallocationtypemapping_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("entitlemententityallocationtypemapping_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("entitlemententityallocationtypemapping_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("entitlemententityallocationtypemapping_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N entitlemententityallocationtypemapping_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("entitlemententityallocationtypemapping_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> entitlemententityallocationtypemapping_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("entitlemententityallocationtypemapping_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("entitlemententityallocationtypemapping_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("entitlemententityallocationtypemapping_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("entitlemententityallocationtypemapping_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("entitlemententityallocationtypemapping_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_entitlemententityallocationtypemapping
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_entitlemententityallocationtypemapping")]
		public Insight.Intake.BusinessUnit business_unit_entitlemententityallocationtypemapping
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_entitlemententityallocationtypemapping", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_entitlemententityallocationtypemapping_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_entitlemententityallocationtypemapping_createdby")]
		public Insight.Intake.SystemUser lk_entitlemententityallocationtypemapping_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_entitlemententityallocationtypemapping_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_entitlemententityallocationtypemapping_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_entitlemententityallocationtypemapping_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_entitlemententityallocationtypemapping_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_entitlemententityallocationtypemapping_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_entitlemententityallocationtypemapping_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_entitlemententityallocationtypemapping_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_entitlemententityallocationtypemapping_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_entitlemententityallocationtypemapping_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_entitlemententityallocationtypemapping_modifiedby")]
		public Insight.Intake.SystemUser lk_entitlemententityallocationtypemapping_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_entitlemententityallocationtypemapping_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_entitlemententityallocationtypemapping_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_entitlemententityallocationtypemapping_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_entitlemententityallocationtypemapping_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_entitlemententityallocationtypemapping_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_entitlemententityallocationtypemapping_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_entitlemententityallocationtypemapping_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_entitlemententityallocationtypemapping_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_entitlemententityallocationtypemapping
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_entitlemententityallocationtypemapping")]
		public Insight.Intake.Team team_entitlemententityallocationtypemapping
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_entitlemententityallocationtypemapping", null);
			}
		}
		
		/// <summary>
		/// N:1 user_entitlemententityallocationtypemapping
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_entitlemententityallocationtypemapping")]
		public Insight.Intake.SystemUser user_entitlemententityallocationtypemapping
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_entitlemententityallocationtypemapping", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public EntitlementEntityAllocationTypeMapping(object anonymousType) : 
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
                        Attributes["entitlemententityallocationtypemappingid"] = base.Id;
                        break;
                    case "entitlemententityallocationtypemappingid":
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