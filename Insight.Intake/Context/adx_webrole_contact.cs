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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("adx_webrole_contact")]
	public partial class adx_webrole_contact : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string adx_webrole_contactId = "adx_webrole_contactid";
			public const string Id = "adx_webrole_contactid";
			public const string adx_webroleid = "adx_webroleid";
			public const string contactid = "contactid";
			public const string VersionNumber = "versionnumber";
		}
		
		public static class Relationships
		{
			public const string adx_webrole_contact1 = "adx_webrole_contact";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public adx_webrole_contact() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "adx_webrole_contact";
		
		public const string EntitySchemaName = "adx_webrole_contact";
		
		public const string PrimaryIdAttribute = "adx_webrole_contactid";
		
		public const string EntityLogicalCollectionName = null;
		
		public const string EntitySetName = "adx_webrole_contactset";
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("adx_webrole_contactid")]
		public System.Nullable<System.Guid> adx_webrole_contactId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("adx_webrole_contactid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("adx_webrole_contactid")]
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("adx_webroleid")]
		public System.Nullable<System.Guid> adx_webroleid
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("adx_webroleid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("contactid")]
		public System.Nullable<System.Guid> contactid
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("contactid");
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
		/// N:N adx_webrole_contact
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("adx_webrole_contact")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.Adx_webrole> adx_webrole_contact1
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.Adx_webrole>("adx_webrole_contact", null);
			}
			set
			{
				this.OnPropertyChanging("adx_webrole_contact1");
				this.SetRelatedEntities<Insight.Intake.Adx_webrole>("adx_webrole_contact", null, value);
				this.OnPropertyChanged("adx_webrole_contact1");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public adx_webrole_contact(object anonymousType) : 
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
                        Attributes["adx_webrole_contactid"] = base.Id;
                        break;
                    case "adx_webrole_contactid":
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