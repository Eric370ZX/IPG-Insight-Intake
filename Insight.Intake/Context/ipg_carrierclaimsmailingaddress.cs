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
	public enum ipg_carrierclaimsmailingaddressState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Entity used to create a single Carrier record for Multiple Claims Mailing Addresses.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_carrierclaimsmailingaddress")]
	public partial class ipg_carrierclaimsmailingaddress : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_addresscarrierclaimmailingId = "ipg_addresscarrierclaimmailingid";
			public const string ipg_carrierclaimname = "ipg_carrierclaimname";
			public const string ipg_carrierclaimsmailingaddressId = "ipg_carrierclaimsmailingaddressid";
			public const string Id = "ipg_carrierclaimsmailingaddressid";
			public const string ipg_claimname = "ipg_claimname";
			public const string ipg_ClaimsAddressZipCodeSelector = "ipg_claimsaddresszipcodeselector";
			public const string ipg_claimsmailingaddress = "ipg_claimsmailingaddress";
			public const string ipg_claimsmailingcity = "ipg_claimsmailingcity";
			public const string ipg_claimsmailingstate = "ipg_claimsmailingstate";
			public const string ipg_ClaimsMailingZipCodeIdId = "ipg_claimsmailingzipcodeidid";
			public const string ipg_electronicpayerid = "ipg_electronicpayerid";
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
			public const string ipg_account_ipg_carrierclaimsmailingaddress = "ipg_account_ipg_carrierclaimsmailingaddress";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_carrierclaimsmailingaddress() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_carrierclaimsmailingaddress";
		
		public const string EntitySchemaName = "ipg_carrierclaimsmailingaddress";
		
		public const string PrimaryIdAttribute = "ipg_carrierclaimsmailingaddressid";
		
		public const string PrimaryNameAttribute = "ipg_claimname";
		
		public const string EntityLogicalCollectionName = "ipg_carrierclaimsmailingaddresses";
		
		public const string EntitySetName = "ipg_carrierclaimsmailingaddresses";
		
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
		/// Unique identifier for Account associated with Carrier Claims Mailing Address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_addresscarrierclaimmailingid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_addresscarrierclaimmailingId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_addresscarrierclaimmailingid");
			}
			set
			{
				this.OnPropertyChanging("ipg_addresscarrierclaimmailingId");
				this.SetAttributeValue("ipg_addresscarrierclaimmailingid", value);
				this.OnPropertyChanged("ipg_addresscarrierclaimmailingId");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierclaimname")]
		public string ipg_carrierclaimname
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_carrierclaimname");
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierclaimname");
				this.SetAttributeValue("ipg_carrierclaimname", value);
				this.OnPropertyChanged("ipg_carrierclaimname");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierclaimsmailingaddressid")]
		public System.Nullable<System.Guid> ipg_carrierclaimsmailingaddressId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_carrierclaimsmailingaddressid");
			}
			set
			{
				this.OnPropertyChanging("ipg_carrierclaimsmailingaddressId");
				this.SetAttributeValue("ipg_carrierclaimsmailingaddressid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_carrierclaimsmailingaddressId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_carrierclaimsmailingaddressid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_carrierclaimsmailingaddressId = value;
			}
		}
		
		/// <summary>
		/// The name of the custom entity.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimname")]
		public string ipg_claimname
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_claimname");
			}
			set
			{
				this.OnPropertyChanging("ipg_claimname");
				this.SetAttributeValue("ipg_claimname", value);
				this.OnPropertyChanged("ipg_claimname");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimsaddresszipcodeselector")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_ClaimsAddressZipCodeSelector
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_claimsaddresszipcodeselector");
			}
			set
			{
				this.OnPropertyChanging("ipg_ClaimsAddressZipCodeSelector");
				this.SetAttributeValue("ipg_claimsaddresszipcodeselector", value);
				this.OnPropertyChanged("ipg_ClaimsAddressZipCodeSelector");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimsaddresszipcodeselector")]
		public virtual ipg_ZipCodeSelector? ipg_ClaimsAddressZipCodeSelectorEnum
		{
			get
			{
				return ((ipg_ZipCodeSelector?)(EntityOptionSetEnum.GetEnum(this, "ipg_claimsaddresszipcodeselector")));
			}
			set
			{
				this.OnPropertyChanging("ipg_ClaimsAddressZipCodeSelector");
				this.SetAttributeValue("ipg_claimsaddresszipcodeselector", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_ClaimsAddressZipCodeSelector");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimsmailingaddress")]
		public string ipg_claimsmailingaddress
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_claimsmailingaddress");
			}
			set
			{
				this.OnPropertyChanging("ipg_claimsmailingaddress");
				this.SetAttributeValue("ipg_claimsmailingaddress", value);
				this.OnPropertyChanged("ipg_claimsmailingaddress");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimsmailingcity")]
		public string ipg_claimsmailingcity
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_claimsmailingcity");
			}
			set
			{
				this.OnPropertyChanging("ipg_claimsmailingcity");
				this.SetAttributeValue("ipg_claimsmailingcity", value);
				this.OnPropertyChanged("ipg_claimsmailingcity");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimsmailingstate")]
		public string ipg_claimsmailingstate
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_claimsmailingstate");
			}
			set
			{
				this.OnPropertyChanging("ipg_claimsmailingstate");
				this.SetAttributeValue("ipg_claimsmailingstate", value);
				this.OnPropertyChanged("ipg_claimsmailingstate");
			}
		}
		
		/// <summary>
		/// Unique identifier for Zip Code associated with Carrier Claims Mailing Address.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_claimsmailingzipcodeidid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_ClaimsMailingZipCodeIdId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_claimsmailingzipcodeidid");
			}
			set
			{
				this.OnPropertyChanging("ipg_ClaimsMailingZipCodeIdId");
				this.SetAttributeValue("ipg_claimsmailingzipcodeidid", value);
				this.OnPropertyChanged("ipg_ClaimsMailingZipCodeIdId");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_electronicpayerid")]
		public string ipg_electronicpayerid
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_electronicpayerid");
			}
			set
			{
				this.OnPropertyChanging("ipg_electronicpayerid");
				this.SetAttributeValue("ipg_electronicpayerid", value);
				this.OnPropertyChanged("ipg_electronicpayerid");
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
		/// Status of the Carrier Claims Mailing Address
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_carrierclaimsmailingaddressState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_carrierclaimsmailingaddressState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_carrierclaimsmailingaddressState), optionSet.Value)));
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
		/// Reason for the status of the Carrier Claims Mailing Address
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
		/// Reason for the status of the Carrier Claims Mailing Address
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_carrierclaimsmailingaddress_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_carrierclaimsmailingaddress_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Incident> ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Incident>("ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress");
				this.SetRelatedEntities<Insight.Intake.Incident>("ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress", null, value);
				this.OnPropertyChanged("ipg_ipg_carrierclaimsmailingaddress_incident_PrimaryCarrierClaimsMailingAddress");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress" +
			"")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Incident> ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Incident>("ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress" +
						"", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress" +
						"");
				this.SetRelatedEntities<Insight.Intake.Incident>("ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress" +
						"", null, value);
				this.OnPropertyChanged("ipg_ipg_carrierclaimsmailingaddress_incident_SecondaryCarrierClaimsMailingAddress" +
						"");
			}
		}
		
		/// <summary>
		/// N:1 ipg_account_ipg_carrierclaimsmailingaddress
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_addresscarrierclaimmailingid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_account_ipg_carrierclaimsmailingaddress")]
		public Insight.Intake.Account ipg_account_ipg_carrierclaimsmailingaddress
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_carrierclaimsmailingaddress", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_account_ipg_carrierclaimsmailingaddress");
				this.SetRelatedEntity<Insight.Intake.Account>("ipg_account_ipg_carrierclaimsmailingaddress", null, value);
				this.OnPropertyChanged("ipg_account_ipg_carrierclaimsmailingaddress");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_carrierclaimsmailingaddress(object anonymousType) : 
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
                        Attributes["ipg_carrierclaimsmailingaddressid"] = base.Id;
                        break;
                    case "ipg_carrierclaimsmailingaddressid":
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