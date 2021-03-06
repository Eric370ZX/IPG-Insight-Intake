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
	/// Customized messages for an entity that has been renamed.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("displaystring")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class DisplayString : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string ComponentState = "componentstate";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string CustomComment = "customcomment";
			public const string CustomDisplayString = "customdisplaystring";
			public const string DisplayStringId = "displaystringid";
			public const string Id = "displaystringid";
			public const string DisplayStringIdUnique = "displaystringidunique";
			public const string DisplayStringKey = "displaystringkey";
			public const string FormatParameters = "formatparameters";
			public const string IsManaged = "ismanaged";
			public const string LanguageCode = "languagecode";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OrganizationId = "organizationid";
			public const string OverwriteTime = "overwritetime";
			public const string PublishedDisplayString = "publisheddisplaystring";
			public const string SolutionId = "solutionid";
			public const string VersionNumber = "versionnumber";
			public const string lk_DisplayStringbase_createdby = "lk_DisplayStringbase_createdby";
			public const string lk_DisplayStringbase_createdonbehalfby = "lk_DisplayStringbase_createdonbehalfby";
			public const string lk_DisplayStringbase_modifiedby = "lk_DisplayStringbase_modifiedby";
			public const string lk_DisplayStringbase_modifiedonbehalfby = "lk_DisplayStringbase_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public DisplayString() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "displaystring";
		
		public const string EntitySchemaName = "DisplayString";
		
		public const string PrimaryIdAttribute = "displaystringid";
		
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
		/// Unique identifier of the user who created the display string.
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
		/// Date and time when the display string was created.
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
		/// Unique identifier of the delegate user who created the displaystring.
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
		/// Comment for a customized display string.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("customcomment")]
		public string CustomComment
		{
			get
			{
				return this.GetAttributeValue<string>("customcomment");
			}
			set
			{
				this.OnPropertyChanging("CustomComment");
				this.SetAttributeValue("customcomment", value);
				this.OnPropertyChanged("CustomComment");
			}
		}
		
		/// <summary>
		/// Customized display string.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("customdisplaystring")]
		public string CustomDisplayString
		{
			get
			{
				return this.GetAttributeValue<string>("customdisplaystring");
			}
			set
			{
				this.OnPropertyChanging("CustomDisplayString");
				this.SetAttributeValue("customdisplaystring", value);
				this.OnPropertyChanged("CustomDisplayString");
			}
		}
		
		/// <summary>
		/// Unique identifier of the display string.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("displaystringid")]
		public System.Nullable<System.Guid> DisplayStringId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("displaystringid");
			}
			set
			{
				this.OnPropertyChanging("DisplayStringId");
				this.SetAttributeValue("displaystringid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("DisplayStringId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("displaystringid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.DisplayStringId = value;
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("displaystringidunique")]
		public System.Nullable<System.Guid> DisplayStringIdUnique
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("displaystringidunique");
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("displaystringkey")]
		public string DisplayStringKey
		{
			get
			{
				return this.GetAttributeValue<string>("displaystringkey");
			}
		}
		
		/// <summary>
		/// Parameters used for formatting the display string.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("formatparameters")]
		public System.Nullable<int> FormatParameters
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("formatparameters");
			}
		}
		
		/// <summary>
		/// 
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
		/// Language code of the display string.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("languagecode")]
		public System.Nullable<int> LanguageCode
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("languagecode");
			}
			set
			{
				this.OnPropertyChanging("LanguageCode");
				this.SetAttributeValue("languagecode", value);
				this.OnPropertyChanged("LanguageCode");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the display string.
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
		/// Date and time when the display string was last modified.
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
		/// Unique identifier of the delegate user who last modified the displaystring.
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
		/// Unique identifier of the organization associated with the display string.
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
		/// Published display string.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("publisheddisplaystring")]
		public string PublishedDisplayString
		{
			get
			{
				return this.GetAttributeValue<string>("publisheddisplaystring");
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
		/// 
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
		/// 1:N DisplayString_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("DisplayString_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> DisplayString_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("DisplayString_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("DisplayString_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("DisplayString_AsyncOperations", null, value);
				this.OnPropertyChanged("DisplayString_AsyncOperations");
			}
		}
		
		/// <summary>
		/// N:1 lk_DisplayStringbase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_DisplayStringbase_createdby")]
		public Insight.Intake.SystemUser lk_DisplayStringbase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_DisplayStringbase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_DisplayStringbase_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_DisplayStringbase_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_DisplayStringbase_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_DisplayStringbase_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_DisplayStringbase_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_DisplayStringbase_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_DisplayStringbase_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_DisplayStringbase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_DisplayStringbase_modifiedby")]
		public Insight.Intake.SystemUser lk_DisplayStringbase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_DisplayStringbase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_DisplayStringbase_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_DisplayStringbase_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_DisplayStringbase_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_DisplayStringbase_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_DisplayStringbase_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_DisplayStringbase_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_DisplayStringbase_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public DisplayString(object anonymousType) : 
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
                        Attributes["displaystringid"] = base.Id;
                        break;
                    case "displaystringid":
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