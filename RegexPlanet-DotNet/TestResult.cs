using System.Runtime.Serialization;

namespace RegexPlanet_DotNet
{
	[DataContract]
	public class TestResult
	{
		[DataMember(Name="success")]
		public bool Success { get; set; }

		[DataMember(Name = "html", EmitDefaultValue = false)]
		public string Html { get; set; }

		[DataMember(Name = "message", EmitDefaultValue = false)]
		public string Message { get; set; }
	}
}