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
	public enum QueueItemState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// A specific item in a queue, such as a case record or an activity record.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("queueitem")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class QueueItem : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string EnteredOn = "enteredon";
			public const string ExchangeRate = "exchangerate";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ObjectId = "objectid";
			public const string ObjectTypeCode = "objecttypecode";
			public const string OrganizationId = "organizationid";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningUser = "owninguser";
			public const string Priority = "priority";
			public const string QueueId = "queueid";
			public const string QueueItemId = "queueitemid";
			public const string Id = "queueitemid";
			public const string Sender = "sender";
			public const string State = "state";
			public const string StateCode = "statecode";
			public const string Status = "status";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string Title = "title";
			public const string ToRecipients = "torecipients";
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string WorkerId = "workerid";
			public const string WorkerIdModifiedOn = "workeridmodifiedon";
			public const string ActivityPointer_QueueItem = "ActivityPointer_QueueItem";
			public const string adx_portalcomment_QueueItems = "adx_portalcomment_QueueItems";
			public const string Appointment_QueueItem = "Appointment_QueueItem";
			public const string Email_QueueItem = "Email_QueueItem";
			public const string Fax_QueueItem = "Fax_QueueItem";
			public const string Incident_QueueItem = "Incident_QueueItem";
			public const string ipg_carrier_audit_history_QueueItems = "ipg_carrier_audit_history_QueueItems";
			public const string ipg_gateactivity_QueueItems = "ipg_gateactivity_QueueItems";
			public const string ipg_historicaudit_QueueItems = "ipg_historicaudit_QueueItems";
			public const string knowledgearticle_QueueItems = "knowledgearticle_QueueItems";
			public const string Letter_QueueItem = "Letter_QueueItem";
			public const string lk_queueitem_createdonbehalfby = "lk_queueitem_createdonbehalfby";
			public const string lk_queueitem_modifiedonbehalfby = "lk_queueitem_modifiedonbehalfby";
			public const string lk_queueitembase_createdby = "lk_queueitembase_createdby";
			public const string lk_queueitembase_modifiedby = "lk_queueitembase_modifiedby";
			public const string lk_queueitembase_workerid = "lk_queueitembase_workerid";
			public const string PhoneCall_QueueItem = "PhoneCall_QueueItem";
			public const string queue_entries = "queue_entries";
			public const string RecurringAppointmentMaster_QueueItem = "RecurringAppointmentMaster_QueueItem";
			public const string ServiceAppointment_QueueItem = "ServiceAppointment_QueueItem";
			public const string Task_QueueItem = "Task_QueueItem";
			public const string team_queueitembase_workerid = "team_queueitembase_workerid";
			public const string TransactionCurrency_QueueItem = "TransactionCurrency_QueueItem";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public QueueItem() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "queueitem";
		
		public const string EntitySchemaName = "QueueItem";
		
		public const string PrimaryIdAttribute = "queueitemid";
		
		public const string PrimaryNameAttribute = "title";
		
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
		/// Shows who created the record.
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
		/// Shows the date and time when the record was created. The date and time are displayed in the time zone selected in Microsoft Dynamics 365 options.
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
		/// Shows the date the record was assigned to the queue.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("enteredon")]
		public System.Nullable<System.DateTime> EnteredOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("enteredon");
			}
		}
		
		/// <summary>
		/// Shows the conversion rate of the record's currency. The exchange rate is used to convert all money fields in the record from the local currency to the system's default currency.
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
		/// Unique identifier of the data import or data migration that created this record.
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
		/// Shows who last updated the record.
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
		/// Shows the date and time when the record was last updated. The date and time are displayed in the time zone selected in Microsoft Dynamics 365 options.
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
		/// Unique identifier of the delegate user who last modified the queueitem.
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
		/// Choose the activity, case, or article assigned to the queue.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		public Microsoft.Xrm.Sdk.EntityReference ObjectId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("objectid");
			}
			set
			{
				this.OnPropertyChanging("ObjectId");
				this.SetAttributeValue("objectid", value);
				this.OnPropertyChanged("ObjectId");
			}
		}
		
		/// <summary>
		/// Select the type of the queue item, such as activity, case, or appointment.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objecttypecode")]
		public Microsoft.Xrm.Sdk.OptionSetValue ObjectTypeCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("objecttypecode");
			}
		}
		
		/// <summary>
		/// Select the type of the queue item, such as activity, case, or appointment.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objecttypecode")]
		public virtual QueueItem_ObjectTypeCode? ObjectTypeCodeEnum
		{
			get
			{
				return ((QueueItem_ObjectTypeCode?)(EntityOptionSetEnum.GetEnum(this, "objecttypecode")));
			}
		}
		
		/// <summary>
		/// Unique identifier of the organization with which the queue item is associated.
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
		/// Unique identifier of the user or team who owns the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ownerid")]
		public Microsoft.Xrm.Sdk.EntityReference OwnerId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ownerid");
			}
		}
		
		/// <summary>
		/// Unique identifier of the business unit that owns the queue item.
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
		/// Unique identifier of the user who owns the queue item.
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
		/// Priority of the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("priority")]
		[System.ObsoleteAttribute()]
		public System.Nullable<int> Priority
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("priority");
			}
			set
			{
				this.OnPropertyChanging("Priority");
				this.SetAttributeValue("priority", value);
				this.OnPropertyChanged("Priority");
			}
		}
		
		/// <summary>
		/// Choose the queue that the item is assigned to.
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
		/// Unique identifier of the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueitemid")]
		public System.Nullable<System.Guid> QueueItemId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("queueitemid");
			}
			set
			{
				this.OnPropertyChanging("QueueItemId");
				this.SetAttributeValue("queueitemid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("QueueItemId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueitemid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.QueueItemId = value;
			}
		}
		
		/// <summary>
		/// Sender who created the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("sender")]
		[System.ObsoleteAttribute()]
		public string Sender
		{
			get
			{
				return this.GetAttributeValue<string>("sender");
			}
			set
			{
				this.OnPropertyChanging("Sender");
				this.SetAttributeValue("sender", value);
				this.OnPropertyChanged("Sender");
			}
		}
		
		/// <summary>
		/// Status of the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("state")]
		[System.ObsoleteAttribute()]
		public System.Nullable<int> State
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("state");
			}
			set
			{
				this.OnPropertyChanging("State");
				this.SetAttributeValue("state", value);
				this.OnPropertyChanged("State");
			}
		}
		
		/// <summary>
		/// Shows whether the queue record is active or inactive. Inactive queue records are read-only and can't be edited unless they are reactivated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.QueueItemState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.QueueItemState)(System.Enum.ToObject(typeof(Insight.Intake.QueueItemState), optionSet.Value)));
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
		/// Reason for the status of the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("status")]
		[System.ObsoleteAttribute()]
		public System.Nullable<int> Status
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("status");
			}
			set
			{
				this.OnPropertyChanging("Status");
				this.SetAttributeValue("status", value);
				this.OnPropertyChanged("Status");
			}
		}
		
		/// <summary>
		/// Select the item's status.
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
		/// Select the item's status.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual QueueItem_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((QueueItem_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// Shows the title or name that describes the queue record. This value is copied from the record that was assigned to the queue.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("title")]
		public string Title
		{
			get
			{
				return this.GetAttributeValue<string>("title");
			}
		}
		
		/// <summary>
		/// Recipients listed on the To line of the message for email queue items.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("torecipients")]
		[System.ObsoleteAttribute()]
		public string ToRecipients
		{
			get
			{
				return this.GetAttributeValue<string>("torecipients");
			}
			set
			{
				this.OnPropertyChanging("ToRecipients");
				this.SetAttributeValue("torecipients", value);
				this.OnPropertyChanged("ToRecipients");
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
			set
			{
				this.OnPropertyChanging("TransactionCurrencyId");
				this.SetAttributeValue("transactioncurrencyid", value);
				this.OnPropertyChanged("TransactionCurrencyId");
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
		/// Version number of the queue item.
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
		/// Shows who is working on the queue item.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("workerid")]
		public Microsoft.Xrm.Sdk.EntityReference WorkerId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("workerid");
			}
			set
			{
				this.OnPropertyChanging("WorkerId");
				this.SetAttributeValue("workerid", value);
				this.OnPropertyChanged("WorkerId");
			}
		}
		
		/// <summary>
		/// Shows the date and time when the queue item was last assigned to a user.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("workeridmodifiedon")]
		public System.Nullable<System.DateTime> WorkerIdModifiedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("workeridmodifiedon");
			}
		}
		
		/// <summary>
		/// 1:N QueueItem_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("QueueItem_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> QueueItem_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("QueueItem_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("QueueItem_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("QueueItem_AsyncOperations", null, value);
				this.OnPropertyChanged("QueueItem_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N queueitem_principalobjectattributeaccess
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("queueitem_principalobjectattributeaccess")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> queueitem_principalobjectattributeaccess
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("queueitem_principalobjectattributeaccess", null);
			}
			set
			{
				this.OnPropertyChanging("queueitem_principalobjectattributeaccess");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("queueitem_principalobjectattributeaccess", null, value);
				this.OnPropertyChanged("queueitem_principalobjectattributeaccess");
			}
		}
		
		/// <summary>
		/// N:1 ActivityPointer_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ActivityPointer_QueueItem")]
		public Insight.Intake.ActivityPointer ActivityPointer_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ActivityPointer>("ActivityPointer_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("ActivityPointer_QueueItem");
				this.SetRelatedEntity<Insight.Intake.ActivityPointer>("ActivityPointer_QueueItem", null, value);
				this.OnPropertyChanged("ActivityPointer_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 adx_portalcomment_QueueItems
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("adx_portalcomment_QueueItems")]
		public Insight.Intake.adx_portalcomment adx_portalcomment_QueueItems
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.adx_portalcomment>("adx_portalcomment_QueueItems", null);
			}
			set
			{
				this.OnPropertyChanging("adx_portalcomment_QueueItems");
				this.SetRelatedEntity<Insight.Intake.adx_portalcomment>("adx_portalcomment_QueueItems", null, value);
				this.OnPropertyChanged("adx_portalcomment_QueueItems");
			}
		}
		
		/// <summary>
		/// N:1 Appointment_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Appointment_QueueItem")]
		public Insight.Intake.Appointment Appointment_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Appointment>("Appointment_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("Appointment_QueueItem");
				this.SetRelatedEntity<Insight.Intake.Appointment>("Appointment_QueueItem", null, value);
				this.OnPropertyChanged("Appointment_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 Email_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Email_QueueItem")]
		public Insight.Intake.Email Email_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Email>("Email_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("Email_QueueItem");
				this.SetRelatedEntity<Insight.Intake.Email>("Email_QueueItem", null, value);
				this.OnPropertyChanged("Email_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 Fax_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Fax_QueueItem")]
		public Insight.Intake.Fax Fax_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Fax>("Fax_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("Fax_QueueItem");
				this.SetRelatedEntity<Insight.Intake.Fax>("Fax_QueueItem", null, value);
				this.OnPropertyChanged("Fax_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 Incident_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Incident_QueueItem")]
		public Insight.Intake.Incident Incident_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Incident>("Incident_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("Incident_QueueItem");
				this.SetRelatedEntity<Insight.Intake.Incident>("Incident_QueueItem", null, value);
				this.OnPropertyChanged("Incident_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 ipg_carrier_audit_history_QueueItems
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_carrier_audit_history_QueueItems")]
		public Insight.Intake.ipg_carrier_audit_history ipg_carrier_audit_history_QueueItems
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_carrier_audit_history>("ipg_carrier_audit_history_QueueItems", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_carrier_audit_history_QueueItems");
				this.SetRelatedEntity<Insight.Intake.ipg_carrier_audit_history>("ipg_carrier_audit_history_QueueItems", null, value);
				this.OnPropertyChanged("ipg_carrier_audit_history_QueueItems");
			}
		}
		
		/// <summary>
		/// N:1 ipg_gateactivity_QueueItems
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_gateactivity_QueueItems")]
		public Insight.Intake.ipg_gateactivity ipg_gateactivity_QueueItems
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_gateactivity>("ipg_gateactivity_QueueItems", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_gateactivity_QueueItems");
				this.SetRelatedEntity<Insight.Intake.ipg_gateactivity>("ipg_gateactivity_QueueItems", null, value);
				this.OnPropertyChanged("ipg_gateactivity_QueueItems");
			}
		}
		
		/// <summary>
		/// N:1 ipg_historicaudit_QueueItems
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_historicaudit_QueueItems")]
		public Insight.Intake.ipg_historicaudit ipg_historicaudit_QueueItems
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_historicaudit>("ipg_historicaudit_QueueItems", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_historicaudit_QueueItems");
				this.SetRelatedEntity<Insight.Intake.ipg_historicaudit>("ipg_historicaudit_QueueItems", null, value);
				this.OnPropertyChanged("ipg_historicaudit_QueueItems");
			}
		}
		
		/// <summary>
		/// N:1 knowledgearticle_QueueItems
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("knowledgearticle_QueueItems")]
		public Insight.Intake.KnowledgeArticle knowledgearticle_QueueItems
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.KnowledgeArticle>("knowledgearticle_QueueItems", null);
			}
			set
			{
				this.OnPropertyChanging("knowledgearticle_QueueItems");
				this.SetRelatedEntity<Insight.Intake.KnowledgeArticle>("knowledgearticle_QueueItems", null, value);
				this.OnPropertyChanged("knowledgearticle_QueueItems");
			}
		}
		
		/// <summary>
		/// N:1 Letter_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Letter_QueueItem")]
		public Insight.Intake.Letter Letter_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Letter>("Letter_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("Letter_QueueItem");
				this.SetRelatedEntity<Insight.Intake.Letter>("Letter_QueueItem", null, value);
				this.OnPropertyChanged("Letter_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 lk_queueitem_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_queueitem_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_queueitem_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitem_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_queueitem_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitem_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_queueitem_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_queueitem_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_queueitem_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_queueitem_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitem_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_queueitem_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitem_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_queueitem_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_queueitembase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_queueitembase_createdby")]
		public Insight.Intake.SystemUser lk_queueitembase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitembase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_queueitembase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_queueitembase_modifiedby")]
		public Insight.Intake.SystemUser lk_queueitembase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitembase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_queueitembase_workerid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("workerid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_queueitembase_workerid")]
		public Insight.Intake.SystemUser lk_queueitembase_workerid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitembase_workerid", null);
			}
			set
			{
				this.OnPropertyChanging("lk_queueitembase_workerid");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_queueitembase_workerid", null, value);
				this.OnPropertyChanged("lk_queueitembase_workerid");
			}
		}
		
		/// <summary>
		/// N:1 PhoneCall_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("PhoneCall_QueueItem")]
		public Insight.Intake.PhoneCall PhoneCall_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.PhoneCall>("PhoneCall_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("PhoneCall_QueueItem");
				this.SetRelatedEntity<Insight.Intake.PhoneCall>("PhoneCall_QueueItem", null, value);
				this.OnPropertyChanged("PhoneCall_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 queue_entries
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("queue_entries")]
		public Insight.Intake.Queue queue_entries
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Queue>("queue_entries", null);
			}
			set
			{
				this.OnPropertyChanging("queue_entries");
				this.SetRelatedEntity<Insight.Intake.Queue>("queue_entries", null, value);
				this.OnPropertyChanged("queue_entries");
			}
		}
		
		/// <summary>
		/// N:1 RecurringAppointmentMaster_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("RecurringAppointmentMaster_QueueItem")]
		public Insight.Intake.RecurringAppointmentMaster RecurringAppointmentMaster_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.RecurringAppointmentMaster>("RecurringAppointmentMaster_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("RecurringAppointmentMaster_QueueItem");
				this.SetRelatedEntity<Insight.Intake.RecurringAppointmentMaster>("RecurringAppointmentMaster_QueueItem", null, value);
				this.OnPropertyChanged("RecurringAppointmentMaster_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 ServiceAppointment_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ServiceAppointment_QueueItem")]
		public Insight.Intake.ServiceAppointment ServiceAppointment_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ServiceAppointment>("ServiceAppointment_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("ServiceAppointment_QueueItem");
				this.SetRelatedEntity<Insight.Intake.ServiceAppointment>("ServiceAppointment_QueueItem", null, value);
				this.OnPropertyChanged("ServiceAppointment_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 Task_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Task_QueueItem")]
		public Insight.Intake.Task Task_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Task>("Task_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("Task_QueueItem");
				this.SetRelatedEntity<Insight.Intake.Task>("Task_QueueItem", null, value);
				this.OnPropertyChanged("Task_QueueItem");
			}
		}
		
		/// <summary>
		/// N:1 team_queueitembase_workerid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("workerid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_queueitembase_workerid")]
		public Insight.Intake.Team team_queueitembase_workerid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Team>("team_queueitembase_workerid", null);
			}
			set
			{
				this.OnPropertyChanging("team_queueitembase_workerid");
				this.SetRelatedEntity<Insight.Intake.Team>("team_queueitembase_workerid", null, value);
				this.OnPropertyChanged("team_queueitembase_workerid");
			}
		}
		
		/// <summary>
		/// N:1 TransactionCurrency_QueueItem
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("TransactionCurrency_QueueItem")]
		public Insight.Intake.TransactionCurrency TransactionCurrency_QueueItem
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.TransactionCurrency>("TransactionCurrency_QueueItem", null);
			}
			set
			{
				this.OnPropertyChanging("TransactionCurrency_QueueItem");
				this.SetRelatedEntity<Insight.Intake.TransactionCurrency>("TransactionCurrency_QueueItem", null, value);
				this.OnPropertyChanged("TransactionCurrency_QueueItem");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public QueueItem(object anonymousType) : 
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
                        Attributes["queueitemid"] = base.Id;
                        break;
                    case "queueitemid":
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