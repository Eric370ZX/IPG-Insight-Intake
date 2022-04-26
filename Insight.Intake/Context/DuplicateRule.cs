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
	public enum DuplicateRuleState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 1,
	}
	
	/// <summary>
	/// Rule used to identify potential duplicates.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("duplicaterule")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class DuplicateRule : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string BaseEntityMatchCodeTable = "baseentitymatchcodetable";
			public const string BaseEntityName = "baseentityname";
			public const string BaseEntityTypeCode = "baseentitytypecode";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string Description = "description";
			public const string DuplicateRuleId = "duplicateruleid";
			public const string Id = "duplicateruleid";
			public const string ExcludeInactiveRecords = "excludeinactiverecords";
			public const string IsCaseSensitive = "iscasesensitive";
			public const string MatchingEntityMatchCodeTable = "matchingentitymatchcodetable";
			public const string MatchingEntityName = "matchingentityname";
			public const string MatchingEntityTypeCode = "matchingentitytypecode";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string BusinessUnit_DuplicateRules = "BusinessUnit_DuplicateRules";
			public const string lk_duplicaterule_createdonbehalfby = "lk_duplicaterule_createdonbehalfby";
			public const string lk_duplicaterule_modifiedonbehalfby = "lk_duplicaterule_modifiedonbehalfby";
			public const string lk_duplicaterulebase_createdby = "lk_duplicaterulebase_createdby";
			public const string lk_duplicaterulebase_modifiedby = "lk_duplicaterulebase_modifiedby";
			public const string SystemUser_DuplicateRules = "SystemUser_DuplicateRules";
			public const string team_DuplicateRules = "team_DuplicateRules";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public DuplicateRule() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "duplicaterule";
		
		public const string EntitySchemaName = "DuplicateRule";
		
		public const string PrimaryIdAttribute = "duplicateruleid";
		
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
		/// Database table that stores match codes for the record type being evaluated for potential duplicates.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("baseentitymatchcodetable")]
		public string BaseEntityMatchCodeTable
		{
			get
			{
				return this.GetAttributeValue<string>("baseentitymatchcodetable");
			}
		}
		
		/// <summary>
		/// Record type of the record being evaluated for potential duplicates.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("baseentityname")]
		public string BaseEntityName
		{
			get
			{
				return this.GetAttributeValue<string>("baseentityname");
			}
			set
			{
				this.OnPropertyChanging("BaseEntityName");
				this.SetAttributeValue("baseentityname", value);
				this.OnPropertyChanged("BaseEntityName");
			}
		}
		
		/// <summary>
		/// Record type of the record being evaluated for potential duplicates.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("baseentitytypecode")]
		public Microsoft.Xrm.Sdk.OptionSetValue BaseEntityTypeCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("baseentitytypecode");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the duplicate detection rule.
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
		/// Date and time when the duplicate detection rule was created.
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
		/// Unique identifier of the delegate user who created the duplicaterule.
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
		/// Description of the duplicate detection rule.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("description")]
		public string Description
		{
			get
			{
				return this.GetAttributeValue<string>("description");
			}
			set
			{
				this.OnPropertyChanging("Description");
				this.SetAttributeValue("description", value);
				this.OnPropertyChanged("Description");
			}
		}
		
		/// <summary>
		/// Unique identifier of the duplicate detection rule.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("duplicateruleid")]
		public System.Nullable<System.Guid> DuplicateRuleId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("duplicateruleid");
			}
			set
			{
				this.OnPropertyChanging("DuplicateRuleId");
				this.SetAttributeValue("duplicateruleid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("DuplicateRuleId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("duplicateruleid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.DuplicateRuleId = value;
			}
		}
		
		/// <summary>
		/// Determines whether to flag inactive records as duplicates
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("excludeinactiverecords")]
		public System.Nullable<bool> ExcludeInactiveRecords
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("excludeinactiverecords");
			}
			set
			{
				this.OnPropertyChanging("ExcludeInactiveRecords");
				this.SetAttributeValue("excludeinactiverecords", value);
				this.OnPropertyChanged("ExcludeInactiveRecords");
			}
		}
		
		/// <summary>
		/// Indicates if the operator is case-sensitive.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("iscasesensitive")]
		public System.Nullable<bool> IsCaseSensitive
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("iscasesensitive");
			}
			set
			{
				this.OnPropertyChanging("IsCaseSensitive");
				this.SetAttributeValue("iscasesensitive", value);
				this.OnPropertyChanged("IsCaseSensitive");
			}
		}
		
		/// <summary>
		/// Database table that stores match codes for potential duplicate records.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("matchingentitymatchcodetable")]
		public string MatchingEntityMatchCodeTable
		{
			get
			{
				return this.GetAttributeValue<string>("matchingentitymatchcodetable");
			}
		}
		
		/// <summary>
		/// Record type of the records being evaluated as potential duplicates.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("matchingentityname")]
		public string MatchingEntityName
		{
			get
			{
				return this.GetAttributeValue<string>("matchingentityname");
			}
			set
			{
				this.OnPropertyChanging("MatchingEntityName");
				this.SetAttributeValue("matchingentityname", value);
				this.OnPropertyChanged("MatchingEntityName");
			}
		}
		
		/// <summary>
		/// Record type of the records being evaluated as potential duplicates.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("matchingentitytypecode")]
		public Microsoft.Xrm.Sdk.OptionSetValue MatchingEntityTypeCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("matchingentitytypecode");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the duplicate detection rule.
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
		/// Date and time when the duplicate detection rule was last modified.
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
		/// Unique identifier of the delegate user who last modified the duplicaterule.
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
		/// Name of the duplicate detection rule.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("name")]
		public string Name
		{
			get
			{
				return this.GetAttributeValue<string>("name");
			}
			set
			{
				this.OnPropertyChanging("Name");
				this.SetAttributeValue("name", value);
				this.OnPropertyChanged("Name");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user or team who owns the duplicate detection rule.
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
		/// Unique identifier of the business unit that owns duplicate detection rule.
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
		/// Unique identifier of the team who owns the duplicate detection rule.
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
		/// Unique identifier of the user who owns the duplicate detection rule.
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
		/// Status of the duplicate detection rule.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.DuplicateRuleState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.DuplicateRuleState)(System.Enum.ToObject(typeof(Insight.Intake.DuplicateRuleState), optionSet.Value)));
				}
				else
				{
					return null;
				}
			}
		}
		
		/// <summary>
		/// Reason for the status of the duplicate detection rule.
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
		/// Reason for the status of the duplicate detection rule.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual DuplicateRule_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((DuplicateRule_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N DuplicateRule_Annotation
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("DuplicateRule_Annotation")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Annotation> DuplicateRule_Annotation
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Annotation>("DuplicateRule_Annotation", null);
			}
			set
			{
				this.OnPropertyChanging("DuplicateRule_Annotation");
				this.SetRelatedEntities<Insight.Intake.Annotation>("DuplicateRule_Annotation", null, value);
				this.OnPropertyChanged("DuplicateRule_Annotation");
			}
		}
		
		/// <summary>
		/// 1:N DuplicateRule_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("DuplicateRule_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> DuplicateRule_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("DuplicateRule_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("DuplicateRule_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("DuplicateRule_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("DuplicateRule_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N DuplicateRule_DuplicateRuleConditions
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("DuplicateRule_DuplicateRuleConditions")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRuleCondition> DuplicateRule_DuplicateRuleConditions
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRuleCondition>("DuplicateRule_DuplicateRuleConditions", null);
			}
			set
			{
				this.OnPropertyChanging("DuplicateRule_DuplicateRuleConditions");
				this.SetRelatedEntities<Insight.Intake.DuplicateRuleCondition>("DuplicateRule_DuplicateRuleConditions", null, value);
				this.OnPropertyChanged("DuplicateRule_DuplicateRuleConditions");
			}
		}
		
		/// <summary>
		/// N:1 BusinessUnit_DuplicateRules
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("BusinessUnit_DuplicateRules")]
		public Insight.Intake.BusinessUnit BusinessUnit_DuplicateRules
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("BusinessUnit_DuplicateRules", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_duplicaterule_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_duplicaterule_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_duplicaterule_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_duplicaterule_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_duplicaterule_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_duplicaterule_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_duplicaterule_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_duplicaterule_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_duplicaterule_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_duplicaterule_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_duplicaterule_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_duplicaterule_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_duplicaterule_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_duplicaterule_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_duplicaterulebase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_duplicaterulebase_createdby")]
		public Insight.Intake.SystemUser lk_duplicaterulebase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_duplicaterulebase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_duplicaterulebase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_duplicaterulebase_modifiedby")]
		public Insight.Intake.SystemUser lk_duplicaterulebase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_duplicaterulebase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 SystemUser_DuplicateRules
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("SystemUser_DuplicateRules")]
		public Insight.Intake.SystemUser SystemUser_DuplicateRules
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("SystemUser_DuplicateRules", null);
			}
		}
		
		/// <summary>
		/// N:1 team_DuplicateRules
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_DuplicateRules")]
		public Insight.Intake.Team team_DuplicateRules
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_DuplicateRules", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public DuplicateRule(object anonymousType) : 
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
                        Attributes["duplicateruleid"] = base.Id;
                        break;
                    case "duplicateruleid":
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