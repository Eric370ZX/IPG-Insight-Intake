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
	/// Defines the individual conditions required for creating records automatically.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("convertruleitem")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ConvertRuleItem : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string ComponentState = "componentstate";
			public const string ConditionId = "conditionid";
			public const string ConditionXml = "conditionxml";
			public const string ConvertRuleId = "convertruleid";
			public const string ConvertRuleItemId = "convertruleitemid";
			public const string Id = "convertruleitemid";
			public const string ConvertRuleItemIdUnique = "convertruleitemidunique";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string Description = "description";
			public const string ExchangeRate = "exchangerate";
			public const string IsManaged = "ismanaged";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OverwriteTime = "overwritetime";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningUser = "owninguser";
			public const string PrimaryCreateEntityLogicalName = "primarycreateentitylogicalname";
			public const string PropertiesXml = "propertiesxml";
			public const string QueueId = "queueid";
			public const string SequenceNumber = "sequencenumber";
			public const string SolutionId = "solutionid";
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string VersionNumber = "versionnumber";
			public const string WorkflowId = "workflowid";
			public const string convertrule_convertruleitem = "convertrule_convertruleitem";
			public const string lk_convertruleitembase_createdby = "lk_convertruleitembase_createdby";
			public const string lk_convertruleitembase_createdonbehalfby = "lk_convertruleitembase_createdonbehalfby";
			public const string lk_convertruleitembase_modifiedby = "lk_convertruleitembase_modifiedby";
			public const string lk_convertruleitembase_modifiedonbehalfby = "lk_convertruleitembase_modifiedonbehalfby";
			public const string queue_convertruleitem = "queue_convertruleitem";
			public const string transactioncurrency_convertruleitem = "transactioncurrency_convertruleitem";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ConvertRuleItem() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "convertruleitem";
		
		public const string EntitySchemaName = "ConvertRuleItem";
		
		public const string PrimaryIdAttribute = "convertruleitemid";
		
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
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("componentstate")]
		public virtual ComponentState? ComponentStateEnum
		{
			get
			{
				return ((ComponentState?)(EntityOptionSetEnum.GetEnum(this, "componentstate")));
			}
		}
		
		/// <summary>
		/// Identifies the step of the associated workflow
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("conditionid")]
		public Microsoft.Xrm.Sdk.EntityReference ConditionId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("conditionid");
			}
			set
			{
				this.OnPropertyChanging("ConditionId");
				this.SetAttributeValue("conditionid", value);
				this.OnPropertyChanged("ConditionId");
			}
		}
		
		/// <summary>
		/// Condition for convert rule item
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("conditionxml")]
		public string ConditionXml
		{
			get
			{
				return this.GetAttributeValue<string>("conditionxml");
			}
			set
			{
				this.OnPropertyChanging("ConditionXml");
				this.SetAttributeValue("conditionxml", value);
				this.OnPropertyChanged("ConditionXml");
			}
		}
		
		/// <summary>
		/// Unique identifier of the convert rule associated with the convert rule item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("convertruleid")]
		public Microsoft.Xrm.Sdk.EntityReference ConvertRuleId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("convertruleid");
			}
			set
			{
				this.OnPropertyChanging("ConvertRuleId");
				this.SetAttributeValue("convertruleid", value);
				this.OnPropertyChanged("ConvertRuleId");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("convertruleitemid")]
		public System.Nullable<System.Guid> ConvertRuleItemId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("convertruleitemid");
			}
			set
			{
				this.OnPropertyChanging("ConvertRuleItemId");
				this.SetAttributeValue("convertruleitemid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ConvertRuleItemId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("convertruleitemid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ConvertRuleItemId = value;
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("convertruleitemidunique")]
		public System.Nullable<System.Guid> ConvertRuleItemIdUnique
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("convertruleitemidunique");
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
		/// Shows who created the record on behalf of another user.
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
		/// Type additional information to describe the rule item for automatic record creation.
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
		/// Exchange rate for the currency associated with the queue with respect to the base currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("exchangerate")]
		public System.Nullable<decimal> ExchangeRate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("exchangerate");
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ismanaged")]
		public System.Nullable<bool> IsManaged
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ismanaged");
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
		/// Shows who created the record on behalf of another user.
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
		/// Type a name or title of the rule item that is used for automatic record creation and update.
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
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overwritetime")]
		public System.Nullable<System.DateTime> OverwriteTime
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overwritetime");
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
		/// Shows the business unit that the convert rule item owner belongs to.
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
		/// Unique identifier of the user who owns the Convert Rule Item.
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
		/// Primary create entity for a rule item
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("primarycreateentitylogicalname")]
		public string PrimaryCreateEntityLogicalName
		{
			get
			{
				return this.GetAttributeValue<string>("primarycreateentitylogicalname");
			}
			set
			{
				this.OnPropertyChanging("PrimaryCreateEntityLogicalName");
				this.SetAttributeValue("primarycreateentitylogicalname", value);
				this.OnPropertyChanged("PrimaryCreateEntityLogicalName");
			}
		}
		
		/// <summary>
		/// Set properties xml for convert rule item
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("propertiesxml")]
		public string PropertiesXml
		{
			get
			{
				return this.GetAttributeValue<string>("propertiesxml");
			}
			set
			{
				this.OnPropertyChanging("PropertiesXml");
				this.SetAttributeValue("propertiesxml", value);
				this.OnPropertyChanged("PropertiesXml");
			}
		}
		
		/// <summary>
		/// Choose the queue that the rule is assigned to.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueid")]
		public Microsoft.Xrm.Sdk.EntityReference QueueId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("queueid");
			}
			set
			{
				this.OnPropertyChanging("QueueId");
				this.SetAttributeValue("queueid", value);
				this.OnPropertyChanged("QueueId");
			}
		}
		
		/// <summary>
		/// Sequence number of the convert rule item
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("sequencenumber")]
		public System.Nullable<int> SequenceNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("sequencenumber");
			}
			set
			{
				this.OnPropertyChanging("SequenceNumber");
				this.SetAttributeValue("sequencenumber", value);
				this.OnPropertyChanged("SequenceNumber");
			}
		}
		
		/// <summary>
		/// Unique identifier of the associated solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
		public System.Nullable<System.Guid> SolutionId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("solutionid");
			}
		}
		
		/// <summary>
		/// Choose the local currency for the record to make sure budgets are reported in the correct currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		public Microsoft.Xrm.Sdk.EntityReference TransactionCurrencyId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("transactioncurrencyid");
			}
		}
		
		/// <summary>
		/// Version number of the Covert Rule Item.
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
		/// Workflow associated with the Convert Rule Item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("workflowid")]
		public Microsoft.Xrm.Sdk.EntityReference WorkflowId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("workflowid");
			}
			set
			{
				this.OnPropertyChanging("WorkflowId");
				this.SetAttributeValue("workflowid", value);
				this.OnPropertyChanged("WorkflowId");
			}
		}
		
		/// <summary>
		/// N:1 convertrule_convertruleitem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("convertruleid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("convertrule_convertruleitem")]
		public Insight.Intake.ConvertRule convertrule_convertruleitem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ConvertRule>("convertrule_convertruleitem", null);
			}
			set
			{
				this.OnPropertyChanging("convertrule_convertruleitem");
				this.SetRelatedEntity<Insight.Intake.ConvertRule>("convertrule_convertruleitem", null, value);
				this.OnPropertyChanged("convertrule_convertruleitem");
			}
		}
		
		/// <summary>
		/// N:1 lk_convertruleitembase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_convertruleitembase_createdby")]
		public Insight.Intake.SystemUser lk_convertruleitembase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_convertruleitembase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_convertruleitembase_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_convertruleitembase_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_convertruleitembase_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_convertruleitembase_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_convertruleitembase_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_convertruleitembase_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_convertruleitembase_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_convertruleitembase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_convertruleitembase_modifiedby")]
		public Insight.Intake.SystemUser lk_convertruleitembase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_convertruleitembase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_convertruleitembase_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_convertruleitembase_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_convertruleitembase_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_convertruleitembase_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_convertruleitembase_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_convertruleitembase_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_convertruleitembase_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 queue_convertruleitem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("queue_convertruleitem")]
		public Insight.Intake.Queue queue_convertruleitem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Queue>("queue_convertruleitem", null);
			}
			set
			{
				this.OnPropertyChanging("queue_convertruleitem");
				this.SetRelatedEntity<Insight.Intake.Queue>("queue_convertruleitem", null, value);
				this.OnPropertyChanged("queue_convertruleitem");
			}
		}
		
		/// <summary>
		/// N:1 transactioncurrency_convertruleitem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("transactioncurrency_convertruleitem")]
		public Insight.Intake.TransactionCurrency transactioncurrency_convertruleitem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.TransactionCurrency>("transactioncurrency_convertruleitem", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ConvertRuleItem(object anonymousType) : 
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
                        Attributes["convertruleitemid"] = base.Id;
                        break;
                    case "convertruleitemid":
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