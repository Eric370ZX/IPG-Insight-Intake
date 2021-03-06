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
	/// Comment on a knowledge base article.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("kbarticlecomment")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.1.0.45")]
	public partial class KbArticleComment : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CommentText = "commenttext";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string KbArticleCommentId = "kbarticlecommentid";
			public const string Id = "kbarticlecommentid";
			public const string KbArticleId = "kbarticleid";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OrganizationId = "organizationid";
			public const string Title = "title";
			public const string VersionNumber = "versionnumber";
			public const string kbarticle_comments = "kbarticle_comments";
			public const string lk_kbarticlecomment_createdonbehalfby = "lk_kbarticlecomment_createdonbehalfby";
			public const string lk_kbarticlecomment_modifiedonbehalfby = "lk_kbarticlecomment_modifiedonbehalfby";
			public const string lk_kbarticlecommentbase_createdby = "lk_kbarticlecommentbase_createdby";
			public const string lk_kbarticlecommentbase_modifiedby = "lk_kbarticlecommentbase_modifiedby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public KbArticleComment() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "kbarticlecomment";
		
		public const string EntitySchemaName = "KbArticleComment";
		
		public const string PrimaryIdAttribute = "kbarticlecommentid";
		
		public const string PrimaryNameAttribute = "title";
		
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
		/// Comment text for the knowledge base article.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("commenttext")]
		public string CommentText
		{
			get
			{
				return this.GetAttributeValue<string>("commenttext");
			}
			set
			{
				this.OnPropertyChanging("CommentText");
				this.SetAttributeValue("commenttext", value);
				this.OnPropertyChanged("CommentText");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the knowledge base article comment.
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
		/// Date and time when the knowledge base article comment was created.
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
		/// Unique identifier of the delegate user who created the kbarticlecomment.
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
		/// Unique identifier of the knowledge base article comment.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("kbarticlecommentid")]
		public System.Nullable<System.Guid> KbArticleCommentId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("kbarticlecommentid");
			}
			set
			{
				this.OnPropertyChanging("KbArticleCommentId");
				this.SetAttributeValue("kbarticlecommentid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("KbArticleCommentId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("kbarticlecommentid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.KbArticleCommentId = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the knowledge base article to which the comment applies.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("kbarticleid")]
		public Microsoft.Xrm.Sdk.EntityReference KbArticleId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("kbarticleid");
			}
			set
			{
				this.OnPropertyChanging("KbArticleId");
				this.SetAttributeValue("kbarticleid", value);
				this.OnPropertyChanged("KbArticleId");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the knowledge base article comment.
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
		/// Date and time when the knowledge base article comment was last modified.
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
		/// Unique identifier of the delegate user who last modified the kbarticlecomment.
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
		/// Unique identifier of the organization with which the article comment is associated.
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
		/// Title of the knowledge base article comment.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("title")]
		public string Title
		{
			get
			{
				return this.GetAttributeValue<string>("title");
			}
			set
			{
				this.OnPropertyChanging("Title");
				this.SetAttributeValue("title", value);
				this.OnPropertyChanged("Title");
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
		/// 1:N KbArticleComment_AsyncOperations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("KbArticleComment_AsyncOperations")]
		public System.Collections.Generic.IEnumerable<Insight.Intake.AsyncOperation> KbArticleComment_AsyncOperations
		{
			get
			{
				return this.GetRelatedEntities<Insight.Intake.AsyncOperation>("KbArticleComment_AsyncOperations", null);
			}
			set
			{
				this.OnPropertyChanging("KbArticleComment_AsyncOperations");
				this.SetRelatedEntities<Insight.Intake.AsyncOperation>("KbArticleComment_AsyncOperations", null, value);
				this.OnPropertyChanged("KbArticleComment_AsyncOperations");
			}
		}
		
		/// <summary>
		/// N:1 kbarticle_comments
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("kbarticleid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("kbarticle_comments")]
		public Insight.Intake.KbArticle kbarticle_comments
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.KbArticle>("kbarticle_comments", null);
			}
			set
			{
				this.OnPropertyChanging("kbarticle_comments");
				this.SetRelatedEntity<Insight.Intake.KbArticle>("kbarticle_comments", null, value);
				this.OnPropertyChanged("kbarticle_comments");
			}
		}
		
		/// <summary>
		/// N:1 lk_kbarticlecomment_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_kbarticlecomment_createdonbehalfby")]
		public Insight.Intake.SystemUser lk_kbarticlecomment_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_kbarticlecomment_createdonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_kbarticlecomment_createdonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_kbarticlecomment_createdonbehalfby", null, value);
				this.OnPropertyChanged("lk_kbarticlecomment_createdonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_kbarticlecomment_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_kbarticlecomment_modifiedonbehalfby")]
		public Insight.Intake.SystemUser lk_kbarticlecomment_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_kbarticlecomment_modifiedonbehalfby", null);
			}
			set
			{
				this.OnPropertyChanging("lk_kbarticlecomment_modifiedonbehalfby");
				this.SetRelatedEntity<Insight.Intake.SystemUser>("lk_kbarticlecomment_modifiedonbehalfby", null, value);
				this.OnPropertyChanged("lk_kbarticlecomment_modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// N:1 lk_kbarticlecommentbase_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_kbarticlecommentbase_createdby")]
		public Insight.Intake.SystemUser lk_kbarticlecommentbase_createdby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_kbarticlecommentbase_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_kbarticlecommentbase_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_kbarticlecommentbase_modifiedby")]
		public Insight.Intake.SystemUser lk_kbarticlecommentbase_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Insight.Intake.SystemUser>("lk_kbarticlecommentbase_modifiedby", null);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		public KbArticleComment(object anonymousType) : 
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
                        Attributes["kbarticlecommentid"] = base.Id;
                        break;
                    case "kbarticlecommentid":
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