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
	/// 
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ipg_ipg_taskreasondetails_ipg_statementgene")]
	public partial class ipg_ipg_taskreasondetails_ipg_statementgene : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string ipg_ipg_taskreasondetails_ipg_statementgeneId = "ipg_ipg_taskreasondetails_ipg_statementgeneid";
			public const string Id = "ipg_ipg_taskreasondetails_ipg_statementgeneid";
			public const string ipg_statementgenerationeventconfigurationid = "ipg_statementgenerationeventconfigurationid";
			public const string ipg_taskreasondetailsid = "ipg_taskreasondetailsid";
			public const string VersionNumber = "versionnumber";
		}
		
		public static class Relationships
		{
			public const string ipg_ipg_taskreasondetails_ipg_statementgenera = "ipg_ipg_taskreasondetails_ipg_statementgenera";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ipg_ipg_taskreasondetails_ipg_statementgene() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "ipg_ipg_taskreasondetails_ipg_statementgene";
		
		public const string EntitySchemaName = "ipg_ipg_taskreasondetails_ipg_statementgene";
		
		public const string PrimaryIdAttribute = "ipg_ipg_taskreasondetails_ipg_statementgeneid";
		
		public const string EntityLogicalCollectionName = null;
		
		public const string EntitySetName = "ipg_ipg_taskreasondetails_ipg_statementgeneset";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_ipg_taskreasondetails_ipg_statementgeneid")]
		public System.Nullable<System.Guid> ipg_ipg_taskreasondetails_ipg_statementgeneId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_ipg_taskreasondetails_ipg_statementgeneid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_ipg_taskreasondetails_ipg_statementgeneid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				base.Id = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_statementgenerationeventconfigurationid")]
		public System.Nullable<System.Guid> ipg_statementgenerationeventconfigurationid
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_statementgenerationeventconfigurationid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ipg_taskreasondetailsid")]
		public System.Nullable<System.Guid> ipg_taskreasondetailsid
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ipg_taskreasondetailsid");
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
		/// N:N ipg_ipg_taskreasondetails_ipg_statementgenera
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ipg_ipg_taskreasondetails_ipg_statementgenera")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.ipg_taskreasondetails> ipg_ipg_taskreasondetails_ipg_statementgenera
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.ipg_taskreasondetails>("ipg_ipg_taskreasondetails_ipg_statementgenera", null);
			}
			set
			{
				this.OnPropertyChanging("ipg_ipg_taskreasondetails_ipg_statementgenera");
				this.SetRelatedEntities<Insight.Intake.ipg_taskreasondetails>("ipg_ipg_taskreasondetails_ipg_statementgenera", null, value);
				this.OnPropertyChanged("ipg_ipg_taskreasondetails_ipg_statementgenera");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public ipg_ipg_taskreasondetails_ipg_statementgene(object anonymousType) : 
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
                        Attributes["ipg_ipg_taskreasondetails_ipg_statementgeneid"] = base.Id;
                        break;
                    case "ipg_ipg_taskreasondetails_ipg_statementgeneid":
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