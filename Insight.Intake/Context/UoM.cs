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
	/// Unit of measure.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("uom")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class UoM : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string BaseUoM = "baseuom";
			public const string CreatedBy = "createdby";
			public const string CreatedByExternalParty = "createdbyexternalparty";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string IsScheduleBaseUoM = "isschedulebaseuom";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedByExternalParty = "modifiedbyexternalparty";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OrganizationId = "organizationid";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string Quantity = "quantity";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UoMId = "uomid";
			public const string Id = "uomid";
			public const string UoMScheduleId = "uomscheduleid";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string lk_externalparty_uom_createdby = "lk_externalparty_uom_createdby";
			public const string lk_externalparty_uom_modifiedby = "lk_externalparty_uom_modifiedby";
			public const string lk_uom_createdonbehalfby = "lk_uom_createdonbehalfby";
			public const string lk_uom_modifiedonbehalfby = "lk_uom_modifiedonbehalfby";
			public const string lk_uombase_createdby = "lk_uombase_createdby";
			public const string lk_uombase_modifiedby = "lk_uombase_modifiedby";
			public const string unit_of_measure_schedule_conversions = "unit_of_measure_schedule_conversions";
			public const string Referencingunit_of_measurement_base_unit = "unit_of_measurement_base_unit";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public UoM() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "uom";
		
		public const string EntitySchemaName = "UoM";
		
		public const string PrimaryIdAttribute = "uomid";
		
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
		/// Choose the base or primary unit on which the unit is based.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("baseuom")]
		public Microsoft.Xrm.Sdk.EntityReference BaseUoM
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("baseuom");
			}
			set
			{
				this.OnPropertyChanging("BaseUoM");
				this.SetAttributeValue("baseuom", value);
				this.OnPropertyChanged("BaseUoM");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the unit.
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
		/// Shows the external party who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyexternalparty")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedByExternalParty
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdbyexternalparty");
			}
		}
		
		/// <summary>
		/// Date and time when the unit was created.
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
		/// Unique identifier of the delegate user who created the uom.
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
		/// Tells whether the unit is the base unit for the associated unit group.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isschedulebaseuom")]
		public System.Nullable<bool> IsScheduleBaseUoM
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("isschedulebaseuom");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the unit.
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
		/// Shows the external party who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyexternalparty")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedByExternalParty
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedbyexternalparty");
			}
		}
		
		/// <summary>
		/// Date and time when the unit was last modified.
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
		/// Unique identifier of the delegate user who last modified the uom.
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
		/// Type a descriptive title or name for the unit of measure.
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
		/// Unique identifier of the organization associated with the unit of measure.
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
		/// Unit quantity for the product.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("quantity")]
		public System.Nullable<decimal> Quantity
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("quantity");
			}
			set
			{
				this.OnPropertyChanging("Quantity");
				this.SetAttributeValue("quantity", value);
				this.OnPropertyChanged("Quantity");
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
		/// Unique identifier of the unit.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("uomid")]
		public System.Nullable<System.Guid> UoMId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("uomid");
			}
			set
			{
				this.OnPropertyChanging("UoMId");
				this.SetAttributeValue("uomid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("UoMId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("uomid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.UoMId = value;
			}
		}
		
		/// <summary>
		/// Choose the ID of the unit group that the unit is associated with.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("uomscheduleid")]
		public Microsoft.Xrm.Sdk.EntityReference UoMScheduleId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("uomscheduleid");
			}
			set
			{
				this.OnPropertyChanging("UoMScheduleId");
				this.SetAttributeValue("uomscheduleid", value);
				this.OnPropertyChanged("UoMScheduleId");
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
		/// 1:N ipg_uom_ipg_casepartdetail_uomid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_uom_ipg_casepartdetail_uomid")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_casepartdetail> ipg_uom_ipg_casepartdetail_uomid
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_casepartdetail>("ipg_uom_ipg_casepartdetail_uomid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_uom_ipg_casepartdetail_uomid");
				this.SetRelatedEntities<Insight.Intake.ipg_casepartdetail>("ipg_uom_ipg_casepartdetail_uomid", null, value);
				this.OnPropertyChanged("ipg_uom_ipg_casepartdetail_uomid");
			}
		}
		
		/// <summary>
		/// 1:N ipg_uom_ipg_estimatedcasepartdetail_uomid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_uom_ipg_estimatedcasepartdetail_uomid")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_estimatedcasepartdetail> ipg_uom_ipg_estimatedcasepartdetail_uomid
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_estimatedcasepartdetail>("ipg_uom_ipg_estimatedcasepartdetail_uomid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_uom_ipg_estimatedcasepartdetail_uomid");
				this.SetRelatedEntities<Insight.Intake.ipg_estimatedcasepartdetail>("ipg_uom_ipg_estimatedcasepartdetail_uomid", null, value);
				this.OnPropertyChanged("ipg_uom_ipg_estimatedcasepartdetail_uomid");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_base_unit
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_base_unit", Microsoft.Xrm.Sdk.EntityRole.Referenced)]
		public System.Collections.Generic.IEnumerable<Insight.Intake.UoM> Referencedunit_of_measurement_base_unit
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.UoM>("unit_of_measurement_base_unit", Microsoft.Xrm.Sdk.EntityRole.Referenced);
			}
			set
			{
				this.OnPropertyChanging("Referencedunit_of_measurement_base_unit");
				this.SetRelatedEntities<Insight.Intake.UoM>("unit_of_measurement_base_unit", Microsoft.Xrm.Sdk.EntityRole.Referenced, value);
				this.OnPropertyChanged("Referencedunit_of_measurement_base_unit");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_invoice_details
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_invoice_details")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.InvoiceDetail> unit_of_measurement_invoice_details
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.InvoiceDetail>("unit_of_measurement_invoice_details", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measurement_invoice_details");
				this.SetRelatedEntities<Insight.Intake.InvoiceDetail>("unit_of_measurement_invoice_details", null, value);
				this.OnPropertyChanged("unit_of_measurement_invoice_details");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_order_details
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_order_details")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.SalesOrderDetail> unit_of_measurement_order_details
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.SalesOrderDetail>("unit_of_measurement_order_details", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measurement_order_details");
				this.SetRelatedEntities<Insight.Intake.SalesOrderDetail>("unit_of_measurement_order_details", null, value);
				this.OnPropertyChanged("unit_of_measurement_order_details");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_product_price_levels
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_product_price_levels")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ProductPriceLevel> unit_of_measurement_product_price_levels
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ProductPriceLevel>("unit_of_measurement_product_price_levels", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measurement_product_price_levels");
				this.SetRelatedEntities<Insight.Intake.ProductPriceLevel>("unit_of_measurement_product_price_levels", null, value);
				this.OnPropertyChanged("unit_of_measurement_product_price_levels");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_productassociation
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_productassociation")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ProductAssociation> unit_of_measurement_productassociation
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ProductAssociation>("unit_of_measurement_productassociation", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measurement_productassociation");
				this.SetRelatedEntities<Insight.Intake.ProductAssociation>("unit_of_measurement_productassociation", null, value);
				this.OnPropertyChanged("unit_of_measurement_productassociation");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_products
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_products")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Product> unit_of_measurement_products
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Product>("unit_of_measurement_products", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measurement_products");
				this.SetRelatedEntities<Insight.Intake.Product>("unit_of_measurement_products", null, value);
				this.OnPropertyChanged("unit_of_measurement_products");
			}
		}
		
		/// <summary>
		/// 1:N unit_of_measurement_quote_details
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_quote_details")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.QuoteDetail> unit_of_measurement_quote_details
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.QuoteDetail>("unit_of_measurement_quote_details", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measurement_quote_details");
				this.SetRelatedEntities<Insight.Intake.QuoteDetail>("unit_of_measurement_quote_details", null, value);
				this.OnPropertyChanged("unit_of_measurement_quote_details");
			}
		}
		
		/// <summary>
		/// 1:N UoM_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("UoM_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> UoM_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("UoM_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("UoM_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("UoM_AsyncOperations", null, value);
				this.OnPropertyChanged("UoM_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N uom_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("uom_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> uom_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("uom_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("uom_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("uom_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("uom_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 lk_externalparty_uom_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyexternalparty")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_externalparty_uom_createdby")]
		public Insight.Intake.ExternalParty lk_externalparty_uom_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ExternalParty>("lk_externalparty_uom_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_externalparty_uom_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyexternalparty")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_externalparty_uom_modifiedby")]
		public Insight.Intake.ExternalParty lk_externalparty_uom_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ExternalParty>("lk_externalparty_uom_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_uom_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_uom_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_uom_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_uom_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_uom_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_uom_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_uom_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_uom_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_uom_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_uom_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_uom_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_uom_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_uom_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_uom_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_uombase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_uombase_createdby")]
		public Insight.Intake.SystemUser lk_uombase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_uombase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_uombase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_uombase_modifiedby")]
		public Insight.Intake.SystemUser lk_uombase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_uombase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 unit_of_measure_schedule_conversions
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("uomscheduleid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measure_schedule_conversions")]
		public Insight.Intake.UoMSchedule unit_of_measure_schedule_conversions
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.UoMSchedule>("unit_of_measure_schedule_conversions", null);
			}
			set
			{
				this.OnPropertyChanging("unit_of_measure_schedule_conversions");
				this.SetRelatedEntity<Insight.Intake.UoMSchedule>("unit_of_measure_schedule_conversions", null, value);
				this.OnPropertyChanged("unit_of_measure_schedule_conversions");
			}
		}
		
		/// <summary>
		/// N:1 unit_of_measurement_base_unit
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("baseuom")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("unit_of_measurement_base_unit", Microsoft.Xrm.Sdk.EntityRole.Referencing)]
		public Insight.Intake.UoM Referencingunit_of_measurement_base_unit
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.UoM>("unit_of_measurement_base_unit", Microsoft.Xrm.Sdk.EntityRole.Referencing);
			}
			set
			{
				this.OnPropertyChanging("Referencingunit_of_measurement_base_unit");
				this.SetRelatedEntity<Insight.Intake.UoM>("unit_of_measurement_base_unit", Microsoft.Xrm.Sdk.EntityRole.Referencing, value);
				this.OnPropertyChanged("Referencingunit_of_measurement_base_unit");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public UoM(object anonymousType) : 
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
                        Attributes["uomid"] = base.Id;
                        break;
                    case "uomid":
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