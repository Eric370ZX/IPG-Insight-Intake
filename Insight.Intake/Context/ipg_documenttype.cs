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
	public enum ipg_documenttypeState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// 
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_documenttype")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class ipg_documenttype : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ipg_DocumentCategoryTypeId = "ipg_documentcategorytypeid";
			public const string ipg_DocumentTypeAbbreviation = "ipg_documenttypeabbreviation";
			public const string ipg_documenttypeId = "ipg_documenttypeid";
			public const string Id = "ipg_documenttypeid";
			public const string ipg_externalid = "ipg_externalid";
			public const string ipg_forportal = "ipg_forportal";
			public const string ipg_name = "ipg_name";
			public const string ipg_Portal = "ipg_portal";
			public const string ipg_PostSurgery = "ipg_postsurgery";
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
			public const string ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId = "ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId";
			public const string lk_ipg_documenttype_createdby = "lk_ipg_documenttype_createdby";
			public const string lk_ipg_documenttype_createdonbehalfby = "lk_ipg_documenttype_createdonbehalfby";
			public const string lk_ipg_documenttype_modifiedby = "lk_ipg_documenttype_modifiedby";
			public const string lk_ipg_documenttype_modifiedonbehalfby = "lk_ipg_documenttype_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_documenttype() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_documenttype";
		
		public const string EntitySchemaName = "ipg_documenttype";
		
		public const string PrimaryIdAttribute = "ipg_documenttypeid";
		
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
		/// The category type that the document belongs to
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_documentcategorytypeid")]
		public Microsoft.Xrm.Sdk.EntityReference ipg_DocumentCategoryTypeId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ipg_documentcategorytypeid");
			}
			set
			{
				this.OnPropertyChanging("ipg_DocumentCategoryTypeId");
				this.SetAttributeValue("ipg_documentcategorytypeid", value);
				this.OnPropertyChanged("ipg_DocumentCategoryTypeId");
			}
		}
		
		/// <summary>
		/// Document Type Abbreviation
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_documenttypeabbreviation")]
		public string ipg_DocumentTypeAbbreviation
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_documenttypeabbreviation");
			}
			set
			{
				this.OnPropertyChanging("ipg_DocumentTypeAbbreviation");
				this.SetAttributeValue("ipg_documenttypeabbreviation", value);
				this.OnPropertyChanged("ipg_DocumentTypeAbbreviation");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_documenttypeid")]
		public System.Nullable<System.Guid> ipg_documenttypeId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_documenttypeid");
			}
			set
			{
				this.OnPropertyChanging("ipg_documenttypeId");
				this.SetAttributeValue("ipg_documenttypeid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("ipg_documenttypeId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_documenttypeid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ipg_documenttypeId = value;
			}
		}
		
		/// <summary>
		/// The external id to store primary key
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_externalid")]
		public string ipg_externalid
		{
			get
			{
				return this.GetAttributeValue<string>("ipg_externalid");
			}
			set
			{
				this.OnPropertyChanging("ipg_externalid");
				this.SetAttributeValue("ipg_externalid", value);
				this.OnPropertyChanged("ipg_externalid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_forportal")]
		public Microsoft.Xrm.Sdk.OptionSetValue ipg_forportal
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("ipg_forportal");
			}
			set
			{
				this.OnPropertyChanging("ipg_forportal");
				this.SetAttributeValue("ipg_forportal", value);
				this.OnPropertyChanged("ipg_forportal");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_forportal")]
		public virtual ipg_documenttype_ipg_forportal? ipg_forportalEnum
		{
			get
			{
				return ((ipg_documenttype_ipg_forportal?)(EntityOptionSetEnum.GetEnum(this, "ipg_forportal")));
			}
			set
			{
				this.OnPropertyChanging("ipg_forportal");
				this.SetAttributeValue("ipg_forportal", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("ipg_forportal");
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
		/// Indicator for whether it should be available in the Portal
		///
		///--- THIS FIELD IS OBSOLETE. DO NOT USE IT. ---
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_portal")]
		public System.Nullable<bool> ipg_Portal
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ipg_portal");
			}
			set
			{
				this.OnPropertyChanging("ipg_Portal");
				this.SetAttributeValue("ipg_portal", value);
				this.OnPropertyChanged("ipg_Portal");
			}
		}
		
		/// <summary>
		/// Indicator for whether a Case Category Doc Type should be visible before Scheduled Date of Service
		///
		///---THIS FIELD IS OBSOLETE. DO NOT USE IT. ---
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_postsurgery")]
		public System.Nullable<bool> ipg_PostSurgery
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ipg_postsurgery");
			}
			set
			{
				this.OnPropertyChanging("ipg_PostSurgery");
				this.SetAttributeValue("ipg_postsurgery", value);
				this.OnPropertyChanged("ipg_PostSurgery");
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
		/// Status of the Document Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Insight.Intake.ipg_documenttypeState> StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Insight.Intake.ipg_documenttypeState)(System.Enum.ToObject(typeof(Insight.Intake.ipg_documenttypeState), optionSet.Value)));
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
		/// Reason for the status of the Document Type
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
		/// Reason for the status of the Document Type
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ipg_documenttype_StatusCode? StatusCodeEnum
		{
			get
			{
				return ((ipg_documenttype_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ipg_documenttype_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_documenttype_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> ipg_documenttype_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_documenttype_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_documenttype_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("ipg_documenttype_AsyncOperations", null, value);
				this.OnPropertyChanged("ipg_documenttype_AsyncOperations");
			}
		}
		
		/// <summary>
		/// 1:N ipg_documenttype_DuplicateBaseRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_documenttype_DuplicateBaseRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_documenttype_DuplicateBaseRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_documenttype_DuplicateBaseRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_documenttype_DuplicateBaseRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_documenttype_DuplicateBaseRecord", null, value);
				this.OnPropertyChanged("ipg_documenttype_DuplicateBaseRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_documenttype_DuplicateMatchingRecord
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_documenttype_DuplicateMatchingRecord")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.DuplicateRecord> ipg_documenttype_DuplicateMatchingRecord
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_documenttype_DuplicateMatchingRecord", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_documenttype_DuplicateMatchingRecord");
				this.SetRelatedEntities<Insight.Intake.DuplicateRecord>("ipg_documenttype_DuplicateMatchingRecord", null, value);
				this.OnPropertyChanged("ipg_documenttype_DuplicateMatchingRecord");
			}
		}
		
		/// <summary>
		/// 1:N ipg_documenttype_PrincipalObjectAttributeAccesses
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_documenttype_PrincipalObjectAttributeAccesses")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.PrincipalObjectAttributeAccess> ipg_documenttype_PrincipalObjectAttributeAccesses
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_documenttype_PrincipalObjectAttributeAccesses", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_documenttype_PrincipalObjectAttributeAccesses");
				this.SetRelatedEntities<Insight.Intake.PrincipalObjectAttributeAccess>("ipg_documenttype_PrincipalObjectAttributeAccesses", null, value);
				this.OnPropertyChanged("ipg_documenttype_PrincipalObjectAttributeAccesses");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_documenttype_ipg_document_DocumentTypeId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documenttype_ipg_document_DocumentTypeId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_document> ipg_ipg_documenttype_ipg_document_DocumentTypeId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_document>("ipg_ipg_documenttype_ipg_document_DocumentTypeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documenttype_ipg_document_DocumentTypeId");
				this.SetRelatedEntities<Insight.Intake.ipg_document>("ipg_ipg_documenttype_ipg_document_DocumentTypeId", null, value);
				this.OnPropertyChanged("ipg_ipg_documenttype_ipg_document_DocumentTypeId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_documenttype_ipg_documentbygate_Documenttype
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documenttype_ipg_documentbygate_Documenttype")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_documentbygate> ipg_ipg_documenttype_ipg_documentbygate_Documenttype
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_documentbygate>("ipg_ipg_documenttype_ipg_documentbygate_Documenttype", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documenttype_ipg_documentbygate_Documenttype");
				this.SetRelatedEntities<Insight.Intake.ipg_documentbygate>("ipg_ipg_documenttype_ipg_documentbygate_Documenttype", null, value);
				this.OnPropertyChanged("ipg_ipg_documenttype_ipg_documentbygate_Documenttype");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_informationtyperequiredinformationrule> ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_informationtyperequiredinformationrule>("ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId");
				this.SetRelatedEntities<Insight.Intake.ipg_informationtyperequiredinformationrule>("ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId", null, value);
				this.OnPropertyChanged("ipg_ipg_documenttype_ipg_informationtyperequiredinformationrule_DocumentTypeId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_requiredinformation> ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_requiredinformation>("ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId");
				this.SetRelatedEntities<Insight.Intake.ipg_requiredinformation>("ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId", null, value);
				this.OnPropertyChanged("ipg_ipg_documenttype_ipg_requiredinformation_DocumentTypeId");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_documenttype_ipg_tasktype_documenttypeid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documenttype_ipg_tasktype_documenttypeid")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_tasktype> ipg_ipg_documenttype_ipg_tasktype_documenttypeid
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_tasktype>("ipg_ipg_documenttype_ipg_tasktype_documenttypeid", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documenttype_ipg_tasktype_documenttypeid");
				this.SetRelatedEntities<Insight.Intake.ipg_tasktype>("ipg_ipg_documenttype_ipg_tasktype_documenttypeid", null, value);
				this.OnPropertyChanged("ipg_ipg_documenttype_ipg_tasktype_documenttypeid");
			}
		}
		
		/// <summary>
		/// 1:N ipg_ipg_documenttype_task_DocumentType
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documenttype_task_DocumentType")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Task> ipg_ipg_documenttype_task_DocumentType
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Task>("ipg_ipg_documenttype_task_DocumentType", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documenttype_task_DocumentType");
				this.SetRelatedEntities<Insight.Intake.Task>("ipg_ipg_documenttype_task_DocumentType", null, value);
				this.OnPropertyChanged("ipg_ipg_documenttype_task_DocumentType");
			}
		}
		
		/// <summary>
		/// N:N ipg_ipg_medrecordsconditions_ipg_documenttype
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_medrecordsconditions_ipg_documenttype")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_medrecordsconditions> ipg_ipg_medrecordsconditions_ipg_documenttype
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_medrecordsconditions>("ipg_ipg_medrecordsconditions_ipg_documenttype", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_medrecordsconditions_ipg_documenttype");
				this.SetRelatedEntities<Insight.Intake.ipg_medrecordsconditions>("ipg_ipg_medrecordsconditions_ipg_documenttype", null, value);
				this.OnPropertyChanged("ipg_ipg_medrecordsconditions_ipg_documenttype");
			}
		}
		
		/// <summary>
		/// N:1 ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_documentcategorytypeid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId")]
		public Insight.Intake.ipg_documentcategorytype ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.ipg_documentcategorytype>("ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId");
				this.SetRelatedEntity<Insight.Intake.ipg_documentcategorytype>("ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId", null, value);
				this.OnPropertyChanged("ipg_ipg_documentcategorytype_ipg_documenttype_DocumentCategoryTypeId");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_documenttype_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_documenttype_createdby")]
		public Insight.Intake.SystemUser lk_ipg_documenttype_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_documenttype_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_documenttype_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_documenttype_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_documenttype_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_documenttype_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_documenttype_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_documenttype_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_documenttype_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_documenttype_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_documenttype_modifiedby")]
		public Insight.Intake.SystemUser lk_ipg_documenttype_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_documenttype_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_ipg_documenttype_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_ipg_documenttype_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_ipg_documenttype_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_documenttype_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_ipg_documenttype_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_ipg_documenttype_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_ipg_documenttype_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_documenttype(object anonymousType) : 
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
                        Attributes["ipg_documenttypeid"] = base.Id;
                        break;
                    case "ipg_documenttypeid":
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