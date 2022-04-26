using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Insight.Intake.Models
{
		[DataContract]
		public class WFDependency
		{
			[DataMember(Name = "entityName")]
			public string EntityName { get; set; }
			[DataMember(Name = "fields")]
			public List<string> Fields { get; set; }
			[DataMember(Name = "fetchCase")]
			public string FetchCase { get; set; }
			[DataMember(Name = "fetchReferral")]
			public string FetchReferral { get; set; }
		}
		

		public class DependencyMetadata
		{
			public WFDependency Dependency { get; set; }
			public EntityReference ProcessId { get; set; }
            public bool AlwaysExecute { get; set; }
    }
}
