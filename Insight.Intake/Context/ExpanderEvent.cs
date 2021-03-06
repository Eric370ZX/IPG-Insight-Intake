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
	
	/// <summary>
	/// For internal use only. An event that will be expanded into jobs whose executions can proceed in the background.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("expanderevent")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ExpanderEvent : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string ContextUri = "contexturi";
			public const string CorrelationId = "correlationid";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ExpanderEventId = "expandereventid";
			public const string Id = "expandereventid";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OrganizationId = "organizationid";
			public const string Registrations = "registrations";
			public const string VersionNumber = "versionnumber";
			public const string createdby_expanderevent = "createdby_expanderevent";
			public const string lk_expanderevent_createdonbehalfby = "lk_expanderevent_createdonbehalfby";
			public const string lk_expanderevent_modifiedonbehalfby = "lk_expanderevent_modifiedonbehalfby";
			public const string modifiedby_expanderevent = "modifiedby_expanderevent";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ExpanderEvent() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "expanderevent";
		
		public const string EntitySchemaName = "ExpanderEvent";
		
		public const string PrimaryIdAttribute = "expandereventid";
		
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
		/// The URI where the context is stored.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("contexturi")]
		public string ContextUri
		{
			get
			{
				return this.GetAttributeValue<string>("contexturi");
			}
			set
			{
				this.OnPropertyChanging("ContextUri");
				this.SetAttributeValue("contexturi", value);
				this.OnPropertyChanged("ContextUri");
			}
		}
		
		/// <summary>
		/// Unique identifier used to correlate between expander events and SDK message invocations.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("correlationid")]
		public System.Nullable<System.Guid> CorrelationId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("correlationid");
			}
			set
			{
				this.OnPropertyChanging("CorrelationId");
				this.SetAttributeValue("correlationid", value);
				this.OnPropertyChanged("CorrelationId");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the event.
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
		/// Date and time when the event was created.
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
		/// Unique identifier of the delegate user who created the event.
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
		/// Unique identifier of the expander event.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("expandereventid")]
		public System.Nullable<System.Guid> ExpanderEventId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("expandereventid");
			}
			set
			{
				this.OnPropertyChanging("ExpanderEventId");
				this.SetAttributeValue("expandereventid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ExpanderEventId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("expandereventid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ExpanderEventId = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the event.
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
		/// Date and time when the event was last modified.
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
		/// Unique identifier of the delegate user who last modified the event.
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
		/// Name of the event.
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
		/// Unique identifier of the organization with which the event is associated.
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
		/// The workloads that have registered to send an event.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("registrations")]
		public string Registrations
		{
			get
			{
				return this.GetAttributeValue<string>("registrations");
			}
			set
			{
				this.OnPropertyChanging("Registrations");
				this.SetAttributeValue("registrations", value);
				this.OnPropertyChanged("Registrations");
			}
		}
		
		/// <summary>
		/// Version number of the event.
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
		/// N:1 createdby_expanderevent
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("createdby_expanderevent")]
		public Insight.Intake.SystemUser createdby_expanderevent
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("createdby_expanderevent", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_expanderevent_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_expanderevent_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_expanderevent_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_expanderevent_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_expanderevent_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_expanderevent_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_expanderevent_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_expanderevent_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_expanderevent_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_expanderevent_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_expanderevent_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_expanderevent_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_expanderevent_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_expanderevent_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 modifiedby_expanderevent
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("modifiedby_expanderevent")]
		public Insight.Intake.SystemUser modifiedby_expanderevent
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("modifiedby_expanderevent", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ExpanderEvent(object anonymousType) : 
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
                        Attributes["expandereventid"] = base.Id;
                        break;
                    case "expandereventid":
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