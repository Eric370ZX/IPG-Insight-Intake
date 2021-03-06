//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Insight.Intake
{
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public enum revokeinheritedaccessrecordstrackerState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("revokeinheritedaccessrecordstracker")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class revokeinheritedaccessrecordstracker : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string RecordsJson = "recordsjson";
			public const string revokeinheritedaccessrecordstracker1 = "revokeinheritedaccessrecordstracker";
			public const string revokeinheritedaccessrecordstrackerId = "revokeinheritedaccessrecordstrackerid";
			public const string Id = "revokeinheritedaccessrecordstrackerid";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string TotalRecords = "totalrecords";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string business_unit_revokeinheritedaccessrecordstracker = "business_unit_revokeinheritedaccessrecordstracker";
			public const string lk_revokeinheritedaccessrecordstracker_createdby = "lk_revokeinheritedaccessrecordstracker_createdby";
			public const string lk_revokeinheritedaccessrecordstracker_createdonbehalfby = "lk_revokeinheritedaccessrecordstracker_createdonbehalfby";
			public const string lk_revokeinheritedaccessrecordstracker_modifiedby = "lk_revokeinheritedaccessrecordstracker_modifiedby";
			public const string lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby = "lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby";
			public const string team_revokeinheritedaccessrecordstracker = "team_revokeinheritedaccessrecordstracker";
			public const string user_revokeinheritedaccessrecordstracker = "user_revokeinheritedaccessrecordstracker";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public revokeinheritedaccessrecordstracker() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "revokeinheritedaccessrecordstracker";
		
		public const string EntitySchemaName = "revokeinheritedaccessrecordstracker";
		
		public const string PrimaryIdAttribute = "revokeinheritedaccessrecordstrackerid";
		
		public const string PrimaryNameAttribute = "revokeinheritedaccessrecordstracker";
		
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
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("recordsjson")]
		public string RecordsJson
		{
			get
			{
				return this.GetAttributeValue<string>("recordsjson");
			}
			set
			{
				this.OnPropertyChanging("RecordsJson");
				this.SetAttributeValue("recordsjson", value);
				this.OnPropertyChanged("RecordsJson");
			}
		}
		
		/// <summary>
		/// The name of the custom entity.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("revokeinheritedaccessrecordstracker")]
		public string revokeinheritedaccessrecordstracker1
		{
			get
			{
				return this.GetAttributeValue<string>("revokeinheritedaccessrecordstracker");
			}
			set
			{
				this.OnPropertyChanging("revokeinheritedaccessrecordstracker1");
				this.SetAttributeValue("revokeinheritedaccessrecordstracker", value);
				this.OnPropertyChanged("revokeinheritedaccessrecordstracker1");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("revokeinheritedaccessrecordstrackerid")]
		public System.Nullable<System.Guid> revokeinheritedaccessrecordstrackerId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("revokeinheritedaccessrecordstrackerid");
			}
			set
			{
				this.OnPropertyChanging("revokeinheritedaccessrecordstrackerId");
				this.SetAttributeValue("revokeinheritedaccessrecordstrackerid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("revokeinheritedaccessrecordstrackerId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("revokeinheritedaccessrecordstrackerid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.revokeinheritedaccessrecordstrackerId = value;
			}
		}
		
		/// <summary>
		/// Status of the RevokeInheritedAccessRecordsTracker
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.revokeinheritedaccessrecordstrackerState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.revokeinheritedaccessrecordstrackerState)(System.Enum.ToObject(typeof(Insight.Intake.revokeinheritedaccessrecordstrackerState), optionSet.Value)));
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
		/// Reason for the status of the RevokeInheritedAccessRecordsTracker
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
		/// Reason for the status of the RevokeInheritedAccessRecordsTracker
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual revokeinheritedaccessrecordstracker_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((revokeinheritedaccessrecordstracker_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("totalrecords")]
		public System.Nullable<int> TotalRecords
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("totalrecords");
			}
			set
			{
				this.OnPropertyChanging("TotalRecords");
				this.SetAttributeValue("totalrecords", value);
				this.OnPropertyChanged("TotalRecords");
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
		/// 1:N revokeinheritedaccessrecordstracker_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("revokeinheritedaccessrecordstracker_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> revokeinheritedaccessrecordstracker_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("revokeinheritedaccessrecordstracker_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("revokeinheritedaccessrecordstracker_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("revokeinheritedaccessrecordstracker_AsyncOperations", null, value);
				this.OnPropertyChanged("revokeinheritedaccessrecordstracker_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N revokeinheritedaccessrecordstracker_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("revokeinheritedaccessrecordstracker_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> revokeinheritedaccessrecordstracker_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("revokeinheritedaccessrecordstracker_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("revokeinheritedaccessrecordstracker_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("revokeinheritedaccessrecordstracker_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("revokeinheritedaccessrecordstracker_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N revokeinheritedaccessrecordstracker_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("revokeinheritedaccessrecordstracker_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> revokeinheritedaccessrecordstracker_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("revokeinheritedaccessrecordstracker_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("revokeinheritedaccessrecordstracker_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("revokeinheritedaccessrecordstracker_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("revokeinheritedaccessrecordstracker_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("revokeinheritedaccessrecordstracker_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_revokeinheritedaccessrecordstracker
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_revokeinheritedaccessrecordstracker")]
		public Insight.Intake.BusinessUnit business_unit_revokeinheritedaccessrecordstracker
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_revokeinheritedaccessrecordstracker", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_revokeinheritedaccessrecordstracker_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_revokeinheritedaccessrecordstracker_createdby")]
		public Insight.Intake.SystemUser lk_revokeinheritedaccessrecordstracker_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_revokeinheritedaccessrecordstracker_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_revokeinheritedaccessrecordstracker_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_revokeinheritedaccessrecordstracker_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_revokeinheritedaccessrecordstracker_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_revokeinheritedaccessrecordstracker_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_revokeinheritedaccessrecordstracker_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_revokeinheritedaccessrecordstracker_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_revokeinheritedaccessrecordstracker_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_revokeinheritedaccessrecordstracker_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_revokeinheritedaccessrecordstracker_modifiedby")]
		public Insight.Intake.SystemUser lk_revokeinheritedaccessrecordstracker_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_revokeinheritedaccessrecordstracker_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_revokeinheritedaccessrecordstracker
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_revokeinheritedaccessrecordstracker")]
		public Insight.Intake.Team team_revokeinheritedaccessrecordstracker
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_revokeinheritedaccessrecordstracker", null);
			}
		}
		
		/// <summary>
		/// N:1 user_revokeinheritedaccessrecordstracker
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_revokeinheritedaccessrecordstracker")]
		public Insight.Intake.SystemUser user_revokeinheritedaccessrecordstracker
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_revokeinheritedaccessrecordstracker", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public revokeinheritedaccessrecordstracker(object anonymousType) : 
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
                        Attributes["revokeinheritedaccessrecordstrackerid"] = base.Id;
                        break;
                    case "revokeinheritedaccessrecordstrackerid":
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