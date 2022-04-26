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
	public enum ipg_dxcptrelationshipState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// This entity holds the N:N relationships between Dx Codes and Cpt codes
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_dxcptrelationship")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_dxcptrelationship : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_CptCodeId = "ipg_cptcodeid";
			public const string ipg_DxCodeId = "ipg_dxcodeid";
			public const string ipg_dxcptrelationshipId = "ipg_dxcptrelationshipid";
			public const string Id = "ipg_dxcptrelationshipid";
			public const string ipg_name = "ipg_name";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OrganizationId = "organizationid";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId = "ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId";
			public const string ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId = "ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId";
			public const string lk_ipg_dxcptrelationship_createdby = "lk_ipg_dxcptrelationship_createdby";
			public const string lk_ipg_dxcptrelationship_createdonbehalfby = "lk_ipg_dxcptrelationship_createdonbehalfby";
			public const string lk_ipg_dxcptrelationship_modifiedby = "lk_ipg_dxcptrelationship_modifiedby";
			public const string lk_ipg_dxcptrelationship_modifiedonbehalfby = "lk_ipg_dxcptrelationship_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_dxcptrelationship() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_dxcptrelationship";
		
		public const string EntitySchemaName = "ipg_dxcptrelationship";
		
		public const string PrimaryIdAttribute = "ipg_dxcptrelationshipid";
		
		public const string PrimaryNameAttribute = "ipg_name";
		
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
		/// The CPT code for this relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_cptcodeid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_CptCodeId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_cptcodeid");
			}
			set
			{
				this.OnPropertyChanging("ipg_CptCodeId");
				this.SetAttributeValue("ipg_cptcodeid", value);
				this.OnPropertyChanged("ipg_CptCodeId");
			}
		}
		
		/// <summary>
		/// The dx code in this relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_dxcodeid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_DxCodeId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_dxcodeid");
			}
			set
			{
				this.OnPropertyChanging("ipg_DxCodeId");
				this.SetAttributeValue("ipg_dxcodeid", value);
				this.OnPropertyChanged("ipg_DxCodeId");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_dxcptrelationshipid")]
		public System.Nullable<System.Guid> ipg_dxcptrelationshipId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_dxcptrelationshipid");
			}
			set
			{
				this.OnPropertyChanging("ipg_dxcptrelationshipId");
				this.SetAttributeValue("ipg_dxcptrelationshipid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_dxcptrelationshipId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_dxcptrelationshipid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_dxcptrelationshipId = value;
			}
		}
		
		/// <summary>
		/// The name of the custom entity.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_name")]
		public string ipg_name
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_name");
			}
			set
			{
				this.OnPropertyChanging("ipg_name");
				this.SetAttributeValue("ipg_name", value);
				this.OnPropertyChanged("ipg_name");
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
		/// Status of the Dx Cpt Relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_dxcptrelationshipState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_dxcptrelationshipState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_dxcptrelationshipState), optionSet.Value)));
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
		/// Reason for the status of the Dx Cpt Relationship
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
		/// Reason for the status of the Dx Cpt Relationship
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_dxcptrelationship_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_dxcptrelationship_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_dxcptrelationship_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_dxcptrelationship_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_dxcptrelationship_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_dxcptrelationship_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_dxcptrelationship_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_dxcptrelationship_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_dxcptrelationship_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_dxcptrelationship_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_dxcptrelationship_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_dxcptrelationship_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_dxcptrelationship_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_dxcptrelationship_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_dxcptrelationship_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_dxcptrelationship_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_dxcptrelationship_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_dxcptrelationship_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_dxcptrelationship_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_dxcptrelationship_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_dxcptrelationship_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_dxcptrelationship_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_dxcptrelationship_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_dxcptrelationship_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_dxcptrelationship_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_dxcptrelationship_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_dxcptrelationship_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_dxcptrelationship_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_dxcptrelationship_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_dxcptrelationship_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_cptcodeid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId")]
		public Insight.Intake.ipg_cptcode ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_cptcode>("ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId");
				this.SetRelatedEntity<Insight.Intake.ipg_cptcode>("ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId", null, value);
				this.OnPropertyChanged("ipg_ipg_cptcode_ipg_dxcptrelationship_CptCodeId");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_dxcodeid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId")]
		public Insight.Intake.ipg_dxcode ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_dxcode>("ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId");
				this.SetRelatedEntity<Insight.Intake.ipg_dxcode>("ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId", null, value);
				this.OnPropertyChanged("ipg_ipg_dxcode_ipg_dxcptrelationship_DxCodeId");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_dxcptrelationship_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_dxcptrelationship_createdby")]
		public Insight.Intake.SystemUser lk_ipg_dxcptrelationship_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_dxcptrelationship_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_dxcptrelationship_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_dxcptrelationship_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_dxcptrelationship_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_dxcptrelationship_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_dxcptrelationship_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_dxcptrelationship_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_dxcptrelationship_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_dxcptrelationship_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_dxcptrelationship_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_dxcptrelationship_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_dxcptrelationship_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_dxcptrelationship_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_dxcptrelationship_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_dxcptrelationship_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_dxcptrelationship_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_dxcptrelationship_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_dxcptrelationship_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_dxcptrelationship_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_dxcptrelationship(object anonymousType) : 
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
                        Attributes["ipg_dxcptrelationshipid"] = base.Id;
                        break;
                    case "ipg_dxcptrelationshipid":
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