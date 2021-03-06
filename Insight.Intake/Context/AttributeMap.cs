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
	/// Represents a mapping between attributes where the attribute values should be copied from a record into the form of a new related record.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("attributemap")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class AttributeMap : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string AttributeMapId = "attributemapid";
			public const string Id = "attributemapid";
			public const string AttributeMapIdUnique = "attributemapidunique";
			public const string ComponentState = "componentstate";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string EntityMapId = "entitymapid";
			public const string IsManaged = "ismanaged";
			public const string IsSystem = "issystem";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OrganizationId = "organizationid";
			public const string OverwriteTime = "overwritetime";
			public const string ParentAttributeMapId = "parentattributemapid";
			public const string SolutionId = "solutionid";
			public const string SourceAttributeName = "sourceattributename";
			public const string TargetAttributeName = "targetattributename";
			public const string VersionNumber = "versionnumber";
			public const string Referencingattribute_map_attribute_maps = "attribute_map_attribute_maps";
			public const string createdby_attributemap = "createdby_attributemap";
			public const string createdonbehalfby_attributemap = "createdonbehalfby_attributemap";
			public const string entity_map_attribute_maps = "entity_map_attribute_maps";
			public const string modifiedby_attributemap = "modifiedby_attributemap";
			public const string modifiedonbehalfby_attributemap = "modifiedonbehalfby_attributemap";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public AttributeMap() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "attributemap";
		
		public const string EntitySchemaName = "AttributeMap";
		
		public const string PrimaryIdAttribute = "attributemapid";
		
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
		/// Unique identifier of the attribute map.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("attributemapid")]
		public System.Nullable<System.Guid> AttributeMapId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("attributemapid");
			}
			set
			{
				this.OnPropertyChanging("AttributeMapId");
				this.SetAttributeValue("attributemapid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("AttributeMapId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("attributemapid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.AttributeMapId = value;
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("attributemapidunique")]
		public System.Nullable<System.Guid> AttributeMapIdUnique
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("attributemapidunique");
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
		/// Unique identifier of the user who created the attribute map.
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
		/// Date and time when the attribute map was created.
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
		/// Unique identifier of the delegate user who created the attributemap.
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
		/// Unique identifier of the entity map with which the attribute map is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitymapid")]
		public Microsoft.Xrm.Sdk.EntityReference EntityMapId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("entitymapid");
			}
			set
			{
				this.OnPropertyChanging("EntityMapId");
				this.SetAttributeValue("entitymapid", value);
				this.OnPropertyChanged("EntityMapId");
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
		/// Information about whether this attribute map is user-defined or system-defined.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("issystem")]
		public System.Nullable<bool> IsSystem
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("issystem");
			}
			set
			{
				this.OnPropertyChanging("IsSystem");
				this.SetAttributeValue("issystem", value);
				this.OnPropertyChanged("IsSystem");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the attribute map.
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
		/// Date and time when the attribute map was last modified.
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
		/// Unique identifier of the delegate user who last modified the attributemap.
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
		/// Unique identifier of the organization with which the attribute map is associated.
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
		/// Unique identifier of the parent attribute map.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentattributemapid")]
		public Microsoft.Xrm.Sdk.EntityReference ParentAttributeMapId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("parentattributemapid");
			}
			set
			{
				this.OnPropertyChanging("ParentAttributeMapId");
				this.SetAttributeValue("parentattributemapid", value);
				this.OnPropertyChanged("ParentAttributeMapId");
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
		/// Name of the source attribute for the mapping.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("sourceattributename")]
		public string SourceAttributeName
		{
			get
			{
				return this.GetAttributeValue<string>("sourceattributename");
			}
			set
			{
				this.OnPropertyChanging("SourceAttributeName");
				this.SetAttributeValue("sourceattributename", value);
				this.OnPropertyChanged("SourceAttributeName");
			}
		}
		
		/// <summary>
		/// Name of the target attribute for the mapping.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("targetattributename")]
		public string TargetAttributeName
		{
			get
			{
				return this.GetAttributeValue<string>("targetattributename");
			}
			set
			{
				this.OnPropertyChanging("TargetAttributeName");
				this.SetAttributeValue("targetattributename", value);
				this.OnPropertyChanged("TargetAttributeName");
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
		/// 1:N attribute_map_attribute_maps
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("attribute_map_attribute_maps", Microsoft.Xrm.Sdk.EntityRole.Referenced)]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AttributeMap> Referencedattribute_map_attribute_maps
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AttributeMap>("attribute_map_attribute_maps", Microsoft.Xrm.Sdk.EntityRole.Referenced);
			}
			set
			{
				this.OnPropertyChanging("Referencedattribute_map_attribute_maps");
				this.SetRelatedEntities<Insight.Intake.AttributeMap>("attribute_map_attribute_maps", Microsoft.Xrm.Sdk.EntityRole.Referenced, value);
				this.OnPropertyChanged("Referencedattribute_map_attribute_maps");
			}
		}
		
		/// <summary>
		/// 1:N AttributeMap_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("AttributeMap_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> AttributeMap_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("AttributeMap_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("AttributeMap_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("AttributeMap_AsyncOperations", null, value);
				this.OnPropertyChanged("AttributeMap_AsyncOperations");
			}
		}
		
		/// <summary>
		/// N:1 attribute_map_attribute_maps
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentattributemapid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("attribute_map_attribute_maps", Microsoft.Xrm.Sdk.EntityRole.Referencing)]
		public Insight.Intake.AttributeMap Referencingattribute_map_attribute_maps
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.AttributeMap>("attribute_map_attribute_maps", Microsoft.Xrm.Sdk.EntityRole.Referencing);
			}
			set
			{
				this.OnPropertyChanging("Referencingattribute_map_attribute_maps");
				this.SetRelatedEntity<Insight.Intake.AttributeMap>("attribute_map_attribute_maps", Microsoft.Xrm.Sdk.EntityRole.Referencing, value);
				this.OnPropertyChanged("Referencingattribute_map_attribute_maps");
			}
		}
		
		/// <summary>
		/// N:1 createdby_attributemap
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("createdby_attributemap")]
		public Insight.Intake.SystemUser createdby_attributemap
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("createdby_attributemap", null);
			}
		}
		
		/// <summary>
		/// N:1 createdonbehalfby_attributemap
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("createdonbehalfby_attributemap")]
		public Insight.Intake.SystemUser createdonbehalfby_attributemap
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("createdonbehalfby_attributemap", null);
			}
			set
			{
				this.OnPropertyChanging("createdonbehalfby_attributemap");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("createdonbehalfby_attributemap", null, value);
				this.OnPropertyChanged("createdonbehalfby_attributemap");
			}
		}
		
		/// <summary>
		/// N:1 entity_map_attribute_maps
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitymapid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("entity_map_attribute_maps")]
		public Insight.Intake.EntityMap entity_map_attribute_maps
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.EntityMap>("entity_map_attribute_maps", null);
			}
			set
			{
				this.OnPropertyChanging("entity_map_attribute_maps");
				this.SetRelatedEntity<Insight.Intake.EntityMap>("entity_map_attribute_maps", null, value);
				this.OnPropertyChanged("entity_map_attribute_maps");
			}
		}
		
		/// <summary>
		/// N:1 modifiedby_attributemap
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("modifiedby_attributemap")]
		public Insight.Intake.SystemUser modifiedby_attributemap
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("modifiedby_attributemap", null);
			}
		}
		
		/// <summary>
		/// N:1 modifiedonbehalfby_attributemap
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("modifiedonbehalfby_attributemap")]
		public Insight.Intake.SystemUser modifiedonbehalfby_attributemap
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("modifiedonbehalfby_attributemap", null);
			}
			set
			{
				this.OnPropertyChanging("modifiedonbehalfby_attributemap");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("modifiedonbehalfby_attributemap", null, value);
				this.OnPropertyChanged("modifiedonbehalfby_attributemap");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public AttributeMap(object anonymousType) : 
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
                        Attributes["attributemapid"] = base.Id;
                        break;
                    case "attributemapid":
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