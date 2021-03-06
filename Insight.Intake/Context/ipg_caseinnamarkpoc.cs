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
	public enum ipg_caseinnamarkpocState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Base entity for process Case Inna Mark POC Lifecycle
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_caseinnamarkpoc")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_caseinnamarkpoc : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string ActiveStageId = "activestageid";
			public const string ActiveStageStartedOn = "activestagestartedon";
			public const string bpf_Duration = "bpf_duration";
			public const string bpf_incidentid = "bpf_incidentid";
			public const string bpf_name = "bpf_name";
			public const string BusinessProcessFlowInstanceId = "businessprocessflowinstanceid";
			public const string Id = "businessprocessflowinstanceid";
			public const string CompletedOn = "completedon";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OrganizationId = "organizationid";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string ProcessId = "processid";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string TraversedPath = "traversedpath";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string bpf_incident_ipg_caseinnamarkpoc = "bpf_incident_ipg_caseinnamarkpoc";
			public const string lk_ipg_caseinnamarkpoc_createdby = "lk_ipg_caseinnamarkpoc_createdby";
			public const string lk_ipg_caseinnamarkpoc_createdonbehalfby = "lk_ipg_caseinnamarkpoc_createdonbehalfby";
			public const string lk_ipg_caseinnamarkpoc_modifiedby = "lk_ipg_caseinnamarkpoc_modifiedby";
			public const string lk_ipg_caseinnamarkpoc_modifiedonbehalfby = "lk_ipg_caseinnamarkpoc_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_caseinnamarkpoc() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_caseinnamarkpoc";
		
		public const string EntitySchemaName = "ipg_caseinnamarkpoc";
		
		public const string PrimaryIdAttribute = "businessprocessflowinstanceid";
		
		public const string PrimaryNameAttribute = "bpf_name";
		
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
		/// Unique identifier of the active stage for the Business Process Flow instance.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activestageid")]
		public Microsoft.Xrm.Sdk.EntityReference ActiveStageId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("activestageid");
			}
			set
			{
				this.OnPropertyChanging("ActiveStageId");
				this.SetAttributeValue("activestageid", value);
				this.OnPropertyChanged("ActiveStageId");
			}
		}
		
		/// <summary>
		/// Date and time when current active stage is started
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activestagestartedon")]
		public System.Nullable<System.DateTime> ActiveStageStartedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("activestagestartedon");
			}
			set
			{
				this.OnPropertyChanging("ActiveStageStartedOn");
				this.SetAttributeValue("activestagestartedon", value);
				this.OnPropertyChanged("ActiveStageStartedOn");
			}
		}
		
		/// <summary>
		/// Duration of Business Process Flow
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("bpf_duration")]
		public System.Nullable<int> bpf_Duration
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("bpf_duration");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("bpf_incidentid")]
		public Microsoft.Xrm.Sdk.EntityReference bpf_incidentid
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("bpf_incidentid");
			}
			set
			{
				this.OnPropertyChanging("bpf_incidentid");
				this.SetAttributeValue("bpf_incidentid", value);
				this.OnPropertyChanged("bpf_incidentid");
			}
		}
		
		/// <summary>
		/// Description
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("bpf_name")]
		public string bpf_name
		{
			get
			{
				return this.GetAttributeValue<string>("bpf_name");
			}
			set
			{
				this.OnPropertyChanging("bpf_name");
				this.SetAttributeValue("bpf_name", value);
				this.OnPropertyChanged("bpf_name");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessprocessflowinstanceid")]
		public System.Nullable<System.Guid> BusinessProcessFlowInstanceId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("businessprocessflowinstanceid");
			}
			set
			{
				this.OnPropertyChanging("BusinessProcessFlowInstanceId");
				this.SetAttributeValue("businessprocessflowinstanceid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("BusinessProcessFlowInstanceId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessprocessflowinstanceid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.BusinessProcessFlowInstanceId = value;
			}
		}
		
		/// <summary>
		/// Date and time when Business Process Flow instance is completed.
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
		/// Unique identifier of the workflow associated to the Business Process Flow instance.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("processid")]
		public Microsoft.Xrm.Sdk.EntityReference ProcessId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("processid");
			}
			set
			{
				this.OnPropertyChanging("ProcessId");
				this.SetAttributeValue("processid", value);
				this.OnPropertyChanged("ProcessId");
			}
		}
		
		/// <summary>
		/// Status of the Case Inna Mark POC Lifecycle
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_caseinnamarkpocState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_caseinnamarkpocState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_caseinnamarkpocState), optionSet.Value)));
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
		/// Reason for the status of the Case Inna Mark POC Lifecycle
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
		/// Comma delimited string of process stage ids that represent visited stages of the Business Process Flow instance.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("traversedpath")]
		public string TraversedPath
		{
			get
			{
				return this.GetAttributeValue<string>("traversedpath");
			}
			set
			{
				this.OnPropertyChanging("TraversedPath");
				this.SetAttributeValue("traversedpath", value);
				this.OnPropertyChanged("TraversedPath");
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
		/// 1:N ipg_caseinnamarkpoc_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_caseinnamarkpoc_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_caseinnamarkpoc_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_caseinnamarkpoc_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_caseinnamarkpoc_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_caseinnamarkpoc_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_caseinnamarkpoc_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_caseinnamarkpoc_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 bpf_incident_ipg_caseinnamarkpoc
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("bpf_incidentid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("bpf_incident_ipg_caseinnamarkpoc")]
		public Insight.Intake.Incident bpf_incident_ipg_caseinnamarkpoc
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Incident>("bpf_incident_ipg_caseinnamarkpoc", null);
			}
			set
			{
				this.OnPropertyChanging("bpf_incident_ipg_caseinnamarkpoc");
				this.SetRelatedEntity<Insight.Intake.Incident>("bpf_incident_ipg_caseinnamarkpoc", null, value);
				this.OnPropertyChanged("bpf_incident_ipg_caseinnamarkpoc");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_caseinnamarkpoc_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_caseinnamarkpoc_createdby")]
		public Insight.Intake.SystemUser lk_ipg_caseinnamarkpoc_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_caseinnamarkpoc_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_caseinnamarkpoc_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_caseinnamarkpoc_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_caseinnamarkpoc_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_caseinnamarkpoc_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_caseinnamarkpoc_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_caseinnamarkpoc_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_caseinnamarkpoc_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_caseinnamarkpoc_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_caseinnamarkpoc_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_caseinnamarkpoc_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_caseinnamarkpoc_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_caseinnamarkpoc_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_caseinnamarkpoc_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_caseinnamarkpoc_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_caseinnamarkpoc_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_caseinnamarkpoc_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_caseinnamarkpoc_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_caseinnamarkpoc_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_caseinnamarkpoc(object anonymousType) : 
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
                        Attributes["businessprocessflowinstanceid"] = base.Id;
                        break;
                    case "businessprocessflowinstanceid":
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