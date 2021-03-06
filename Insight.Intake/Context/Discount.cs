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
	/// Price reduction made from the list price of a product or service based on the quantity purchased.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("discount")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class Discount : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string Amount = "amount";
			public const string Amount_Base = "amount_base";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string DiscountId = "discountid";
			public const string Id = "discountid";
			public const string DiscountTypeId = "discounttypeid";
			public const string ExchangeRate = "exchangerate";
			public const string HighQuantity = "highquantity";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string IsAmountType = "isamounttype";
			public const string LowQuantity = "lowquantity";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OrganizationId = "organizationid";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string Percentage = "percentage";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string discount_type_discounts = "discount_type_discounts";
			public const string lk_discount_createdonbehalfby = "lk_discount_createdonbehalfby";
			public const string lk_discount_modifiedonbehalfby = "lk_discount_modifiedonbehalfby";
			public const string lk_discountbase_createdby = "lk_discountbase_createdby";
			public const string lk_discountbase_modifiedby = "lk_discountbase_modifiedby";
			public const string transactioncurrency_discount = "transactioncurrency_discount";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Discount() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "discount";
		
		public const string EntitySchemaName = "Discount";
		
		public const string PrimaryIdAttribute = "discountid";
		
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
		/// Amount of the discount, specified either as a percentage or as a monetary amount.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("amount")]
		public Microsoft.Xrm.Sdk.Money Amount
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("amount");
			}
			set
			{
				this.OnPropertyChanging("Amount");
				this.SetAttributeValue("amount", value);
				this.OnPropertyChanged("Amount");
			}
		}
		
		/// <summary>
		/// Value of the Amount in base currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("amount_base")]
		public Microsoft.Xrm.Sdk.Money Amount_Base
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.Money>("amount_base");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the discount.
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
		/// Date and time when the discount was created.
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
		/// Unique identifier of the delegate user who created the discount.
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
		/// Unique identifier of the discount.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("discountid")]
		public System.Nullable<System.Guid> DiscountId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("discountid");
			}
			set
			{
				this.OnPropertyChanging("DiscountId");
				this.SetAttributeValue("discountid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("DiscountId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("discountid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.DiscountId = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the discount list associated with the discount.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("discounttypeid")]
		public Microsoft.Xrm.Sdk.EntityReference DiscountTypeId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("discounttypeid");
			}
			set
			{
				this.OnPropertyChanging("DiscountTypeId");
				this.SetAttributeValue("discounttypeid", value);
				this.OnPropertyChanged("DiscountTypeId");
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
		/// Upper boundary for the quantity range to which a particular discount can be applied.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("highquantity")]
		public System.Nullable<decimal> HighQuantity
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("highquantity");
			}
			set
			{
				this.OnPropertyChanging("HighQuantity");
				this.SetAttributeValue("highquantity", value);
				this.OnPropertyChanged("HighQuantity");
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
		/// Specifies whether the discount is specified as a monetary amount or a percentage.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isamounttype")]
		public System.Nullable<bool> IsAmountType
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("isamounttype");
			}
		}
		
		/// <summary>
		/// Lower boundary for the quantity range to which a particular discount is applied.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("lowquantity")]
		public System.Nullable<decimal> LowQuantity
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("lowquantity");
			}
			set
			{
				this.OnPropertyChanging("LowQuantity");
				this.SetAttributeValue("lowquantity", value);
				this.OnPropertyChanged("LowQuantity");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the discount.
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
		/// Date and time when the discount was last modified.
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
		/// Unique identifier of the delegate user who last modified the discount.
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
		/// name
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
		/// Unique identifier of the organization associated with the discount.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationid")]
		public System.Nullable<System.Guid> OrganizationId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("organizationid");
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
		/// Percentage discount value.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("percentage")]
		public System.Nullable<decimal> Percentage
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("percentage");
			}
			set
			{
				this.OnPropertyChanging("Percentage");
				this.SetAttributeValue("percentage", value);
				this.OnPropertyChanged("Percentage");
			}
		}
		
		/// <summary>
		/// Select the discount's status.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		[System.ObsoleteAttribute()]
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
		/// 1:N Discount_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("Discount_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> Discount_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("Discount_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("Discount_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("Discount_AsyncOperations", null, value);
				this.OnPropertyChanged("Discount_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N discount_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("discount_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> discount_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("discount_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("discount_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("discount_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("discount_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 discount_type_discounts
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("discounttypeid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("discount_type_discounts")]
		public Insight.Intake.DiscountType discount_type_discounts
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.DiscountType>("discount_type_discounts", null);
			}
			set
			{
				this.OnPropertyChanging("discount_type_discounts");
				this.SetRelatedEntity<Insight.Intake.DiscountType>("discount_type_discounts", null, value);
				this.OnPropertyChanged("discount_type_discounts");
			}
		}
		
		/// <summary>
		/// N:1 lk_discount_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_discount_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_discount_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_discount_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_discount_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_discount_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_discount_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_discount_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_discount_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_discount_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_discount_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_discount_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_discount_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_discount_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_discountbase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_discountbase_createdby")]
		public Insight.Intake.SystemUser lk_discountbase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_discountbase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_discountbase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_discountbase_modifiedby")]
		public Insight.Intake.SystemUser lk_discountbase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_discountbase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 transactioncurrency_discount
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("transactioncurrency_discount")]
		public Insight.Intake.TransactionCurrency transactioncurrency_discount
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.TransactionCurrency>("transactioncurrency_discount", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public Discount(object anonymousType) : 
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
                        Attributes["discountid"] = base.Id;
                        break;
                    case "discountid":
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