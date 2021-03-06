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
	public enum ipg_taskreasonState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_taskreason")]
	public partial class ipg_taskreason : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_description = "ipg_description";
			public const string ipg_identificator = "ipg_identificator";
			public const string ipg_name = "ipg_name";
			public const string ipg_rules = "ipg_rules";
			public const string ipg_taskcategory = "ipg_taskcategory";
			public const string ipg_TaskReasonCode = "ipg_taskreasoncode";
			public const string ipg_taskreasonId = "ipg_taskreasonid";
			public const string Id = "ipg_taskreasonid";
			public const string ipg_tasktype = "ipg_tasktype";
			public const string ipg_usedbyactioncodes = "ipg_usedbyactioncodes";
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
			public const string ipg_ipg_tasktype_ipg_taskreason_tasktype = "ipg_ipg_tasktype_ipg_taskreason_tasktype";
		}
		
		public static class Relationships
		{
			public const string ipg_ipg_taskreasondetails_ipg_taskreason = "ipg_ipg_taskreasondetails_ipg_taskreason";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_taskreason() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_taskreason";
		
		public const string EntitySchemaName = "ipg_taskreason";
		
		public const string PrimaryIdAttribute = "ipg_taskreasonid";
		
		public const string PrimaryNameAttribute = "ipg_name";
		
		public const string EntityLogicalCollectionName = "ipg_taskreasons";
		
		public const string EntitySetName = "ipg_taskreasons";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_description")]
		public string ipg_description
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_description");
			}
			set
			{
				this.OnPropertyChanging("ipg_description");
				this.SetAttributeValue("ipg_description", value);
				this.OnPropertyChanged("ipg_description");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_identificator")]
		public string ipg_identificator
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_identificator");
			}
			set
			{
				this.OnPropertyChanging("ipg_identificator");
				this.SetAttributeValue("ipg_identificator", value);
				this.OnPropertyChanged("ipg_identificator");
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_rules")]
		public string ipg_rules
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_rules");
			}
			set
			{
				this.OnPropertyChanging("ipg_rules");
				this.SetAttributeValue("ipg_rules", value);
				this.OnPropertyChanged("ipg_rules");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taskcategory")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_taskcategory
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_taskcategory");
			}
			set
			{
				this.OnPropertyChanging("ipg_taskcategory");
				this.SetAttributeValue("ipg_taskcategory", value);
				this.OnPropertyChanged("ipg_taskcategory");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taskreasoncode")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_TaskReasonCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_taskreasoncode");
			}
			set
			{
				this.OnPropertyChanging("ipg_TaskReasonCode");
				this.SetAttributeValue("ipg_taskreasoncode", value);
				this.OnPropertyChanged("ipg_TaskReasonCode");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taskreasoncode")]
		public virtual ipg_TaskReasons? ipg_TaskReasonCodeEnum
		{
			get
			{
				return ((ipg_TaskReasons?)(EntityOptionSetEnum.GetEnum(this, "ipg_taskreasoncode")));
			}
			set
			{
				this.OnPropertyChanging("ipg_TaskReasonCode");
				this.SetAttributeValue("ipg_taskreasoncode", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_TaskReasonCode");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taskreasonid")]
		public System.Nullable<System.Guid> ipg_taskreasonId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_taskreasonid");
			}
			set
			{
				this.OnPropertyChanging("ipg_taskreasonId");
				this.SetAttributeValue("ipg_taskreasonid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_taskreasonId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taskreasonid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_taskreasonId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_tasktype")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_tasktype
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_tasktype");
			}
			set
			{
				this.OnPropertyChanging("ipg_tasktype");
				this.SetAttributeValue("ipg_tasktype", value);
				this.OnPropertyChanged("ipg_tasktype");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_usedbyactioncodes")]
		public Microsoft.Xrm.Sdk.OptionSetValueCollection ipg_usedbyactioncodes
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValueCollection>("ipg_usedbyactioncodes");
			}
			set
			{
				this.OnPropertyChanging("ipg_usedbyactioncodes");
				this.SetAttributeValue("ipg_usedbyactioncodes", value);
				this.OnPropertyChanged("ipg_usedbyactioncodes");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_usedbyactioncodes")]
		public virtual System.Collections.Generic.IEnumerable<ipg_taskreason_ipg_UsedByActionCodes> ipg_usedbyactioncodesEnum
		{
			get
			{
				return EntityOptionSetEnum.GetMultiEnum<ipg_taskreason_ipg_UsedByActionCodes>(this, "ipg_usedbyactioncodes");
			}
			set
			{
				this.OnPropertyChanging("ipg_usedbyactioncodes");
				this.SetAttributeValue("ipg_usedbyactioncodes", EntityOptionSetEnum.GetMultiEnum(this, "ipg_usedbyactioncodes", value));
				this.OnPropertyChanged("ipg_usedbyactioncodes");
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
		/// Status of the Task Reason
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_taskreasonState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_taskreasonState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_taskreasonState), optionSet.Value)));
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
		/// Reason for the status of the Task Reason
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
		/// Reason for the status of the Task Reason
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_taskreason_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_taskreason_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_taskreasondetails> ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_taskreasondetails>("ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid");
				this.SetRelatedEntities<Insight.Intake.ipg_taskreasondetails>("ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid", null, value);
				this.OnPropertyChanged("ipg_ipg_taskreason_ipg_taskreasondetails_taskreasonid");
			}
		}
		
		/// <summary>
		/// N:N ipg_ipg_taskreasondetails_ipg_taskreason
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_taskreasondetails_ipg_taskreason")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_taskreasondetails> ipg_ipg_taskreasondetails_ipg_taskreason
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_taskreasondetails>("ipg_ipg_taskreasondetails_ipg_taskreason", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_taskreasondetails_ipg_taskreason");
				this.SetRelatedEntities<Insight.Intake.ipg_taskreasondetails>("ipg_ipg_taskreasondetails_ipg_taskreason", null, value);
				this.OnPropertyChanged("ipg_ipg_taskreasondetails_ipg_taskreason");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_tasktype_ipg_taskreason_tasktype
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_tasktype")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_tasktype_ipg_taskreason_tasktype")]
		public Insight.Intake.ipg_tasktype ipg_ipg_tasktype_ipg_taskreason_tasktype
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_tasktype>("ipg_ipg_tasktype_ipg_taskreason_tasktype", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_tasktype_ipg_taskreason_tasktype");
				this.SetRelatedEntity<Insight.Intake.ipg_tasktype>("ipg_ipg_tasktype_ipg_taskreason_tasktype", null, value);
				this.OnPropertyChanged("ipg_ipg_tasktype_ipg_taskreason_tasktype");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_taskreason(object anonymousType) : 
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
                        Attributes["ipg_taskreasonid"] = base.Id;
                        break;
                    case "ipg_taskreasonid":
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