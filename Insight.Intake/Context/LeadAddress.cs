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
	/// Address information for a lead.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("leadaddress")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class LeadAddress : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string AddressNumber = "addressnumber";
			public const string AddressTypeCode = "addresstypecode";
			public const string City = "city";
			public const string Composite = "composite";
			public const string Country = "country";
			public const string County = "county";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ExchangeRate = "exchangerate";
			public const string Fax = "fax";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string Latitude = "latitude";
			public const string LeadAddressId = "leadaddressid";
			public const string Id = "leadaddressid";
			public const string Line1 = "line1";
			public const string Line2 = "line2";
			public const string Line3 = "line3";
			public const string Longitude = "longitude";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string Name = "name";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string ParentId = "parentid";
			public const string PostalCode = "postalcode";
			public const string PostOfficeBox = "postofficebox";
			public const string ShippingMethodCode = "shippingmethodcode";
			public const string StateOrProvince = "stateorprovince";
			public const string Telephone1 = "telephone1";
			public const string Telephone2 = "telephone2";
			public const string Telephone3 = "telephone3";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string UPSZone = "upszone";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string UTCOffset = "utcoffset";
			public const string VersionNumber = "versionnumber";
			public const string lead_addresses = "lead_addresses";
			public const string lk_leadaddress_createdonbehalfby = "lk_leadaddress_createdonbehalfby";
			public const string lk_leadaddress_modifiedonbehalfby = "lk_leadaddress_modifiedonbehalfby";
			public const string lk_leadaddressbase_createdby = "lk_leadaddressbase_createdby";
			public const string lk_leadaddressbase_modifiedby = "lk_leadaddressbase_modifiedby";
			public const string TransactionCurrency_LeadAddress = "TransactionCurrency_LeadAddress";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public LeadAddress() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "leadaddress";
		
		public const string EntitySchemaName = "LeadAddress";
		
		public const string PrimaryIdAttribute = "leadaddressid";
		
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
		/// Information about the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("addressnumber")]
		public System.Nullable<int> AddressNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("addressnumber");
			}
			set
			{
				this.OnPropertyChanging("AddressNumber");
				this.SetAttributeValue("addressnumber", value);
				this.OnPropertyChanged("AddressNumber");
			}
		}
		
		/// <summary>
		/// Type of address for the lead address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("addresstypecode")]
		public Microsoft.Xrm.Sdk.OptionSetValue AddressTypeCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("addresstypecode");
			}
			set
			{
				this.OnPropertyChanging("AddressTypeCode");
				this.SetAttributeValue("addresstypecode", value);
				this.OnPropertyChanged("AddressTypeCode");
			}
		}
		
		/// <summary>
		/// City name in the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("city")]
		public string City
		{
			get
			{
				return this.GetAttributeValue<string>("city");
			}
			set
			{
				this.OnPropertyChanging("City");
				this.SetAttributeValue("city", value);
				this.OnPropertyChanged("City");
			}
		}
		
		/// <summary>
		/// Shows the complete address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("composite")]
		public string Composite
		{
			get
			{
				return this.GetAttributeValue<string>("composite");
			}
		}
		
		/// <summary>
		/// Country/region name in the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("country")]
		public string Country
		{
			get
			{
				return this.GetAttributeValue<string>("country");
			}
			set
			{
				this.OnPropertyChanging("Country");
				this.SetAttributeValue("country", value);
				this.OnPropertyChanged("Country");
			}
		}
		
		/// <summary>
		/// County name in the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("county")]
		public string County
		{
			get
			{
				return this.GetAttributeValue<string>("county");
			}
			set
			{
				this.OnPropertyChanging("County");
				this.SetAttributeValue("county", value);
				this.OnPropertyChanged("County");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the lead address.
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
		/// Date and time when the lead address was created.
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
		/// Unique identifier of the delegate user who created the leadaddress.
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
		/// Exchange rate for the currency associated with the leadaddress with respect to the base currency.
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
		/// Fax number for the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("fax")]
		public string Fax
		{
			get
			{
				return this.GetAttributeValue<string>("fax");
			}
			set
			{
				this.OnPropertyChanging("Fax");
				this.SetAttributeValue("fax", value);
				this.OnPropertyChanged("Fax");
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
		/// Latitude for the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("latitude")]
		public System.Nullable<double> Latitude
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<double>>("latitude");
			}
			set
			{
				this.OnPropertyChanging("Latitude");
				this.SetAttributeValue("latitude", value);
				this.OnPropertyChanged("Latitude");
			}
		}
		
		/// <summary>
		/// Unique identifier of the lead address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("leadaddressid")]
		public System.Nullable<System.Guid> LeadAddressId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("leadaddressid");
			}
			set
			{
				this.OnPropertyChanging("LeadAddressId");
				this.SetAttributeValue("leadaddressid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("LeadAddressId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("leadaddressid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.LeadAddressId = value;
			}
		}
		
		/// <summary>
		/// First line for entering address information.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("line1")]
		public string Line1
		{
			get
			{
				return this.GetAttributeValue<string>("line1");
			}
			set
			{
				this.OnPropertyChanging("Line1");
				this.SetAttributeValue("line1", value);
				this.OnPropertyChanged("Line1");
			}
		}
		
		/// <summary>
		/// Second line for entering address information.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("line2")]
		public string Line2
		{
			get
			{
				return this.GetAttributeValue<string>("line2");
			}
			set
			{
				this.OnPropertyChanging("Line2");
				this.SetAttributeValue("line2", value);
				this.OnPropertyChanged("Line2");
			}
		}
		
		/// <summary>
		/// Third line for entering address information.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("line3")]
		public string Line3
		{
			get
			{
				return this.GetAttributeValue<string>("line3");
			}
			set
			{
				this.OnPropertyChanging("Line3");
				this.SetAttributeValue("line3", value);
				this.OnPropertyChanged("Line3");
			}
		}
		
		/// <summary>
		/// Longitude for the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("longitude")]
		public System.Nullable<double> Longitude
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<double>>("longitude");
			}
			set
			{
				this.OnPropertyChanging("Longitude");
				this.SetAttributeValue("longitude", value);
				this.OnPropertyChanged("Longitude");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the lead address.
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
		/// Date and time when the lead address was last modified.
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
		/// Unique identifier of the delegate user who last modified the leadaddress.
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
		/// Name used to identify the lead address.
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
		/// Unique identifier of the parent object with which the lead address is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentid")]
		public Microsoft.Xrm.Sdk.EntityReference ParentId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("parentid");
			}
			set
			{
				this.OnPropertyChanging("ParentId");
				this.SetAttributeValue("parentid", value);
				this.OnPropertyChanged("ParentId");
			}
		}
		
		/// <summary>
		/// ZIP Code or postal code in the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("postalcode")]
		public string PostalCode
		{
			get
			{
				return this.GetAttributeValue<string>("postalcode");
			}
			set
			{
				this.OnPropertyChanging("PostalCode");
				this.SetAttributeValue("postalcode", value);
				this.OnPropertyChanged("PostalCode");
			}
		}
		
		/// <summary>
		/// Post office box number in the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("postofficebox")]
		public string PostOfficeBox
		{
			get
			{
				return this.GetAttributeValue<string>("postofficebox");
			}
			set
			{
				this.OnPropertyChanging("PostOfficeBox");
				this.SetAttributeValue("postofficebox", value);
				this.OnPropertyChanged("PostOfficeBox");
			}
		}
		
		/// <summary>
		/// Method of shipment for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("shippingmethodcode")]
		public Microsoft.Xrm.Sdk.OptionSetValue ShippingMethodCode
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("shippingmethodcode");
			}
			set
			{
				this.OnPropertyChanging("ShippingMethodCode");
				this.SetAttributeValue("shippingmethodcode", value);
				this.OnPropertyChanged("ShippingMethodCode");
			}
		}
		
		/// <summary>
		/// State or province in the address for the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("stateorprovince")]
		public string StateOrProvince
		{
			get
			{
				return this.GetAttributeValue<string>("stateorprovince");
			}
			set
			{
				this.OnPropertyChanging("StateOrProvince");
				this.SetAttributeValue("stateorprovince", value);
				this.OnPropertyChanged("StateOrProvince");
			}
		}
		
		/// <summary>
		/// First telephone number for the lead address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("telephone1")]
		public string Telephone1
		{
			get
			{
				return this.GetAttributeValue<string>("telephone1");
			}
			set
			{
				this.OnPropertyChanging("Telephone1");
				this.SetAttributeValue("telephone1", value);
				this.OnPropertyChanged("Telephone1");
			}
		}
		
		/// <summary>
		/// Second telephone number for the lead address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("telephone2")]
		public string Telephone2
		{
			get
			{
				return this.GetAttributeValue<string>("telephone2");
			}
			set
			{
				this.OnPropertyChanging("Telephone2");
				this.SetAttributeValue("telephone2", value);
				this.OnPropertyChanged("Telephone2");
			}
		}
		
		/// <summary>
		/// Third telephone number for the lead address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("telephone3")]
		public string Telephone3
		{
			get
			{
				return this.GetAttributeValue<string>("telephone3");
			}
			set
			{
				this.OnPropertyChanging("Telephone3");
				this.SetAttributeValue("telephone3", value);
				this.OnPropertyChanged("Telephone3");
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
		/// Unique identifier of the currency associated with the leadaddress.
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
		/// United Parcel Service (UPS) zone for the address of the lead.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("upszone")]
		public string UPSZone
		{
			get
			{
				return this.GetAttributeValue<string>("upszone");
			}
			set
			{
				this.OnPropertyChanging("UPSZone");
				this.SetAttributeValue("upszone", value);
				this.OnPropertyChanged("UPSZone");
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
		/// UTC offset for the lead address. This is the difference between local time and standard Coordinated Universal Time.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("utcoffset")]
		public System.Nullable<int> UTCOffset
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("utcoffset");
			}
			set
			{
				this.OnPropertyChanging("UTCOffset");
				this.SetAttributeValue("utcoffset", value);
				this.OnPropertyChanged("UTCOffset");
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
		/// 1:N leadaddress_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("leadaddress_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> leadaddress_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("leadaddress_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("leadaddress_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("leadaddress_AsyncOperations", null, value);
				this.OnPropertyChanged("leadaddress_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N leadaddress_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("leadaddress_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> leadaddress_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("leadaddress_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("leadaddress_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("leadaddress_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("leadaddress_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 lead_addresses
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lead_addresses")]
		public Insight.Intake.Lead lead_addresses
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Lead>("lead_addresses", null);
			}
			set
			{
				this.OnPropertyChanging("lead_addresses");
				this.SetRelatedEntity<Insight.Intake.Lead>("lead_addresses", null, value);
				this.OnPropertyChanged("lead_addresses");
			}
		}
		
		/// <summary>
		/// N:1 lk_leadaddress_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_leadaddress_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_leadaddress_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_leadaddress_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_leadaddress_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_leadaddress_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_leadaddress_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_leadaddress_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_leadaddress_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_leadaddress_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_leadaddress_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_leadaddress_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_leadaddress_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_leadaddress_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_leadaddressbase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_leadaddressbase_createdby")]
		public Insight.Intake.SystemUser lk_leadaddressbase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_leadaddressbase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_leadaddressbase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_leadaddressbase_modifiedby")]
		public Insight.Intake.SystemUser lk_leadaddressbase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_leadaddressbase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 TransactionCurrency_LeadAddress
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("TransactionCurrency_LeadAddress")]
		public Insight.Intake.TransactionCurrency TransactionCurrency_LeadAddress
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.TransactionCurrency>("TransactionCurrency_LeadAddress", null);
			}
			set
			{
				this.OnPropertyChanging("TransactionCurrency_LeadAddress");
				this.SetRelatedEntity<Insight.Intake.TransactionCurrency>("TransactionCurrency_LeadAddress", null, value);
				this.OnPropertyChanged("TransactionCurrency_LeadAddress");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public LeadAddress(object anonymousType) : 
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
                        Attributes["leadaddressid"] = base.Id;
                        break;
                    case "leadaddressid":
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