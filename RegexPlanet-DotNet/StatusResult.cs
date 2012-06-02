using System.Runtime.Serialization;

namespace RegexPlanet_DotNet
{
	[DataContract]
	public class StatusResult
	{
		[DataMember(Name="success")]
		public bool Success { get; set; }

		[DataMember(Name = "test", EmitDefaultValue = false)]
		public string Test { get; set; }

		[DataMember(Name = "message", EmitDefaultValue = false)]
		public string Message { get; set; }

		[DataMember(Name = "System.Environment.Version", EmitDefaultValue = false)]
		public string SystemEnvironmentVersion { get; set; }
	}
}
