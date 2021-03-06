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
	public enum flowsessionState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Entity to store the information that is generated when a modern flow or UI flow runs.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("flowsession")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class flowsession : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CompletedOn = "completedon";
			public const string Context = "context";
			public const string CorrelationId = "correlationid";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ErrorCode = "errorcode";
			public const string ErrorMessage = "errormessage";
			public const string flowsessionId = "flowsessionid";
			public const string Id = "flowsessionid";
			public const string Gateway = "gateway";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string ProcessVersion = "processversion";
			public const string RegardingObjectId = "regardingobjectid";
			public const string StartedOn = "startedon";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string business_unit_flowsession = "business_unit_flowsession";
			public const string lk_flowsession_createdby = "lk_flowsession_createdby";
			public const string lk_flowsession_createdonbehalfby = "lk_flowsession_createdonbehalfby";
			public const string lk_flowsession_modifiedby = "lk_flowsession_modifiedby";
			public const string lk_flowsession_modifiedonbehalfby = "lk_flowsession_modifiedonbehalfby";
			public const string team_flowsession = "team_flowsession";
			public const string user_flowsession = "user_flowsession";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public flowsession() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "flowsession";
		
		public const string EntitySchemaName = "flowsession";
		
		public const string PrimaryIdAttribute = "flowsessionid";
		
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
		/// The date and time at which the flow session was completed.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("completedon")]
		public System.Nullable<System.DateTime> CompletedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("completedon");
			}
			set
			{
				this.OnPropertyChanging("CompletedOn");
				this.SetAttributeValue("completedon", value);
				this.OnPropertyChanged("CompletedOn");
			}
		}
		
		/// <summary>
		/// Context about flow session.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("context")]
		public string Context
		{
			get
			{
				return this.GetAttributeValue<string>("context");
			}
			set
			{
				this.OnPropertyChanging("Context");
				this.SetAttributeValue("context", value);
				this.OnPropertyChanged("Context");
			}
		}
		
		/// <summary>
		/// Unique identifier used to correlate between multiple SDK requests and flow executions.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("correlationid")]
		public string CorrelationId
		{
			get
			{
				return this.GetAttributeValue<string>("correlationid");
			}
			set
			{
				this.OnPropertyChanging("CorrelationId");
				this.SetAttributeValue("correlationid", value);
				this.OnPropertyChanged("CorrelationId");
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
		/// Error code describing the failure in flow session execution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("errorcode")]
		public string ErrorCode
		{
			get
			{
				return this.GetAttributeValue<string>("errorcode");
			}
			set
			{
				this.OnPropertyChanging("ErrorCode");
				this.SetAttributeValue("errorcode", value);
				this.OnPropertyChanged("ErrorCode");
			}
		}
		
		/// <summary>
		/// Message describing the failure in flow session execution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("errormessage")]
		public string ErrorMessage
		{
			get
			{
				return this.GetAttributeValue<string>("errormessage");
			}
			set
			{
				this.OnPropertyChanging("ErrorMessage");
				this.SetAttributeValue("errormessage", value);
				this.OnPropertyChanged("ErrorMessage");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("flowsessionid")]
		public System.Nullable<System.Guid> flowsessionId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("flowsessionid");
			}
			set
			{
				this.OnPropertyChanging("flowsessionId");
				this.SetAttributeValue("flowsessionid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("flowsessionId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("flowsessionid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.flowsessionId = value;
			}
		}
		
		/// <summary>
		/// The name of the gateway used.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("gateway")]
		public string Gateway
		{
			get
			{
				return this.GetAttributeValue<string>("gateway");
			}
			set
			{
				this.OnPropertyChanging("Gateway");
				this.SetAttributeValue("gateway", value);
				this.OnPropertyChanged("Gateway");
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
		/// The version of the process with which the flow session is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("processversion")]
		public string ProcessVersion
		{
			get
			{
				return this.GetAttributeValue<string>("processversion");
			}
			set
			{
				this.OnPropertyChanging("ProcessVersion");
				this.SetAttributeValue("processversion", value);
				this.OnPropertyChanged("ProcessVersion");
			}
		}
		
		/// <summary>
		/// Unique identifier of the process with which the flow session is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("regardingobjectid")]
		public Microsoft.Xrm.Sdk.EntityReference RegardingObjectId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("regardingobjectid");
			}
			set
			{
				this.OnPropertyChanging("RegardingObjectId");
				this.SetAttributeValue("regardingobjectid", value);
				this.OnPropertyChanged("RegardingObjectId");
			}
		}
		
		/// <summary>
		/// The date and time at which the flow session was started.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("startedon")]
		public System.Nullable<System.DateTime> StartedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("startedon");
			}
			set
			{
				this.OnPropertyChanging("StartedOn");
				this.SetAttributeValue("startedon", value);
				this.OnPropertyChanged("StartedOn");
			}
		}
		
		/// <summary>
		/// Status of the FlowSession
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.flowsessionState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.flowsessionState)(System.Enum.ToObject(typeof(Insight.Intake.flowsessionState), optionSet.Value)));
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
		/// Reason for the status of the FlowSession
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
		/// Reason for the status of the FlowSession
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual flowsession_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((flowsession_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N flowsession_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("flowsession_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> flowsession_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("flowsession_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("flowsession_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("flowsession_AsyncOperations", null, value);
				this.OnPropertyChanged("flowsession_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N flowsession_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("flowsession_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> flowsession_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("flowsession_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("flowsession_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("flowsession_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("flowsession_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_flowsession
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_flowsession")]
		public Insight.Intake.BusinessUnit business_unit_flowsession
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.BusinessUnit>("business_unit_flowsession", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_flowsession_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_flowsession_createdby")]
		public Insight.Intake.SystemUser lk_flowsession_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_flowsession_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_flowsession_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_flowsession_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_flowsession_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_flowsession_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_flowsession_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_flowsession_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_flowsession_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_flowsession_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_flowsession_modifiedby")]
		public Insight.Intake.SystemUser lk_flowsession_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_flowsession_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_flowsession_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_flowsession_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_flowsession_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_flowsession_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_flowsession_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_flowsession_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_flowsession_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 team_flowsession
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_flowsession")]
		public Insight.Intake.Team team_flowsession
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_flowsession", null);
			}
		}
		
		/// <summary>
		/// N:1 user_flowsession
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("user_flowsession")]
		public Insight.Intake.SystemUser user_flowsession
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("user_flowsession", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public flowsession(object anonymousType) : 
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
                        Attributes["flowsessionid"] = base.Id;
                        break;
                    case "flowsessionid":
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