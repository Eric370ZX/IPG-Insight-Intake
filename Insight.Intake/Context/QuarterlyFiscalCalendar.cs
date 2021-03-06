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
	/// Quarterly fiscal calendar of an organization. A span of time during which the financial activities of an organization are calculated.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("quarterlyfiscalcalendar")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class QuarterlyFiscalCalendar : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string BusinessUnitId = "businessunitid";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string EffectiveOn = "effectiveon";
			public const string ExchangeRate = "exchangerate";
			public const string FiscalPeriodType = "fiscalperiodtype";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Quarter1 = "quarter1";
			public const string Quarter1_Base = "quarter1_base";
			public const string Quarter2 = "quarter2";
			public const string Quarter2_Base = "quarter2_base";
			public const string Quarter3 = "quarter3";
			public const string Quarter3_Base = "quarter3_base";
			public const string Quarter4 = "quarter4";
			public const string Quarter4_Base = "quarter4_base";
			public const string SalesPersonId = "salespersonid";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string UserFiscalCalendarId = "userfiscalcalendarid";
			public const string Id = "userfiscalcalendarid";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string lk_quarterlyfiscalcalendar_createdby = "lk_quarterlyfiscalcalendar_createdby";
			public const string lk_quarterlyfiscalcalendar_createdonbehalfby = "lk_quarterlyfiscalcalendar_createdonbehalfby";
			public const string lk_quarterlyfiscalcalendar_modifiedby = "lk_quarterlyfiscalcalendar_modifiedby";
			public const string lk_quarterlyfiscalcalendar_modifiedonbehalfby = "lk_quarterlyfiscalcalendar_modifiedonbehalfby";
			public const string lk_quarterlyfiscalcalendar_salespersonid = "lk_quarterlyfiscalcalendar_salespersonid";
			public const string transactioncurrency_quarterlyfiscalcalendar = "transactioncurrency_quarterlyfiscalcalendar";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public QuarterlyFiscalCalendar() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "quarterlyfiscalcalendar";
		
		public const string EntitySchemaName = "QuarterlyFiscalCalendar";
		
		public const string PrimaryIdAttribute = "userfiscalcalendarid";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitid")]
		public Microsoft.Xrm.Sdk.EntityReference BusinessUnitId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("businessunitid");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the quarterly fiscal calendar.
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
		/// Date and time when the quota for the quarterly fiscal calendar was created.
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
		/// Unique identifier of the delegate user who created the quarterlyfiscalcalendar.
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
		/// Date and time when the quarterly fiscal calendar sales quota takes effect.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("effectiveon")]
		public System.Nullable<System.DateTime> EffectiveOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("effectiveon");
			}
			set
			{
				this.OnPropertyChanging("EffectiveOn");
				this.SetAttributeValue("effectiveon", value);
				this.OnPropertyChanged("EffectiveOn");
			}
		}
		
		/// <summary>
		/// Exchange rate for the currency associated with the quarterly fiscal calendar with respect to the base currency.
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
		/// Type of fiscal period used in the sales quota.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("fiscalperiodtype")]
		public System.Nullable<int> FiscalPeriodType
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("fiscalperiodtype");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the quarterly fiscal calendar.
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
		/// Date and time when the quarterly fiscal calendar was last modified.
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
		/// Unique identifier of the delegate user who last modified the quarterlyfiscalcalendar.
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
		/// Sales quota for the first quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter1")]
		public Microsoft.Xrm.Sdk.Money Quarter1
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter1");
			}
			set
			{
				this.OnPropertyChanging("Quarter1");
				this.SetAttributeValue("quarter1", value);
				this.OnPropertyChanged("Quarter1");
			}
		}
		
		/// <summary>
		/// Base currency equivalent of the sales quota for the first quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter1_base")]
		public Microsoft.Xrm.Sdk.Money Quarter1_Base
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter1_base");
			}
		}
		
		/// <summary>
		/// Sales quota for the second quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter2")]
		public Microsoft.Xrm.Sdk.Money Quarter2
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter2");
			}
			set
			{
				this.OnPropertyChanging("Quarter2");
				this.SetAttributeValue("quarter2", value);
				this.OnPropertyChanged("Quarter2");
			}
		}
		
		/// <summary>
		/// Base currency equivalent of the sales quota for the second quarter in the fiscal year
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter2_base")]
		public Microsoft.Xrm.Sdk.Money Quarter2_Base
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter2_base");
			}
		}
		
		/// <summary>
		/// Sales quota for the third quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter3")]
		public Microsoft.Xrm.Sdk.Money Quarter3
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter3");
			}
			set
			{
				this.OnPropertyChanging("Quarter3");
				this.SetAttributeValue("quarter3", value);
				this.OnPropertyChanged("Quarter3");
			}
		}
		
		/// <summary>
		/// Base currency equivalent of the sales quota for the third quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter3_base")]
		public Microsoft.Xrm.Sdk.Money Quarter3_Base
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter3_base");
			}
		}
		
		/// <summary>
		/// Sales quota for the fourth quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter4")]
		public Microsoft.Xrm.Sdk.Money Quarter4
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter4");
			}
			set
			{
				this.OnPropertyChanging("Quarter4");
				this.SetAttributeValue("quarter4", value);
				this.OnPropertyChanged("Quarter4");
			}
		}
		
		/// <summary>
		/// Base currency equivalent of the sales quota for the fourth quarter in the fiscal year.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quarter4_base")]
		public Microsoft.Xrm.Sdk.Money Quarter4_Base
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("quarter4_base");
			}
		}
		
		/// <summary>
		/// Unique identifier of the associated salesperson.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("salespersonid")]
		public Microsoft.Xrm.Sdk.EntityReference SalesPersonId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("salespersonid");
			}
			set
			{
				this.OnPropertyChanging("SalesPersonId");
				this.SetAttributeValue("salespersonid", value);
				this.OnPropertyChanged("SalesPersonId");
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
		/// Unique identifier of the currency associated with the quarterly fiscal calendar.
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
		/// Unique identifier of the quarterly fiscal calendar.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("userfiscalcalendarid")]
		public System.Nullable<System.Guid> UserFiscalCalendarId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("userfiscalcalendarid");
			}
			set
			{
				this.OnPropertyChanging("UserFiscalCalendarId");
				this.SetAttributeValue("userfiscalcalendarid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("UserFiscalCalendarId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("userfiscalcalendarid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.UserFiscalCalendarId = value;
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
		/// 1:N QuarterlyFiscalCalendar_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("QuarterlyFiscalCalendar_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> QuarterlyFiscalCalendar_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("QuarterlyFiscalCalendar_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("QuarterlyFiscalCalendar_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("QuarterlyFiscalCalendar_AsyncOperations", null, value);
				this.OnPropertyChanged("QuarterlyFiscalCalendar_AsyncOperations");
			}
		}
		
		/// <summary>
		/// N:1 lk_quarterlyfiscalcalendar_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_quarterlyfiscalcalendar_createdby")]
		public Insight.Intake.SystemUser lk_quarterlyfiscalcalendar_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_quarterlyfiscalcalendar_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_quarterlyfiscalcalendar_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_quarterlyfiscalcalendar_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_quarterlyfiscalcalendar_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_quarterlyfiscalcalendar_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_quarterlyfiscalcalendar_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_quarterlyfiscalcalendar_modifiedby")]
		public Insight.Intake.SystemUser lk_quarterlyfiscalcalendar_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_quarterlyfiscalcalendar_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_quarterlyfiscalcalendar_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_quarterlyfiscalcalendar_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_quarterlyfiscalcalendar_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_quarterlyfiscalcalendar_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_quarterlyfiscalcalendar_salespersonid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("salespersonid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_quarterlyfiscalcalendar_salespersonid")]
		public Insight.Intake.SystemUser lk_quarterlyfiscalcalendar_salespersonid
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_salespersonid", null);
			}
			set
			{
				this.OnPropertyChanging("lk_quarterlyfiscalcalendar_salespersonid");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_quarterlyfiscalcalendar_salespersonid", null, value);
				this.OnPropertyChanged("lk_quarterlyfiscalcalendar_salespersonid");
			}
		}
		
		/// <summary>
		/// N:1 transactioncurrency_quarterlyfiscalcalendar
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("transactioncurrency_quarterlyfiscalcalendar")]
		public Insight.Intake.TransactionCurrency transactioncurrency_quarterlyfiscalcalendar
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.TransactionCurrency>("transactioncurrency_quarterlyfiscalcalendar", null);
			}
			set
			{
				this.OnPropertyChanging("transactioncurrency_quarterlyfiscalcalendar");
				this.SetRelatedEntity<Insight.Intake.TransactionCurrency>("transactioncurrency_quarterlyfiscalcalendar", null, value);
				this.OnPropertyChanged("transactioncurrency_quarterlyfiscalcalendar");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public QuarterlyFiscalCalendar(object anonymousType) : 
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
                        Attributes["userfiscalcalendarid"] = base.Id;
                        break;
                    case "userfiscalcalendarid":
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