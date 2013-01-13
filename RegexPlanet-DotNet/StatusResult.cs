using System.Runtime.Serialization;

namespace RegexPlanet_DotNet
{
	[DataContract]
	public class StatusResult
	{
		[DataMember(Name="success")]
		public bool Success { get; set; }

		[DataMember(Name="version")]
		public string Version { get; set; }

		[DataMember(Name = "System.Environment.CommandLine", EmitDefaultValue = false)]
		public string SystemEnvironmentCommandLine { get; set; }

		[DataMember(Name = "System.Environment.CurrentDirectory", EmitDefaultValue = false)]
		public string SystemEnvironmentCurrentDirectory { get; set; }

		[DataMember(Name = "System.Environment.Is64BitOperatingSystem", EmitDefaultValue = false)]
		public bool SystemEnvironmentIs64BitOperatingSystem { get; set; }

		[DataMember(Name = "System.Environment.Is64BitProcess", EmitDefaultValue = false)]
		public bool SystemEnvironmentIs64BitProcess { get; set; }

		[DataMember(Name = "System.Environment.MachineName", EmitDefaultValue = false)]
		public string SystemEnvironmentMachineName { get; set; }

		[DataMember(Name = "System.Environment.OSVersion", EmitDefaultValue = false)]
		public string SystemEnvironmentOSVersion { get; set; }

		[DataMember(Name = "System.Environment.ProcessorCount", EmitDefaultValue = false)]
		public int SystemEnvironmentProcessorCount { get; set; }

		[DataMember(Name = "System.Environment.SystemDirectory", EmitDefaultValue = false)]
		public string SystemEnvironmentSystemDirectory { get; set; }

		[DataMember(Name = "System.Environment.SystemPageSize", EmitDefaultValue = false)]
		public int SystemEnvironmentSystemPageSize { get; set; }

		[DataMember(Name = "System.Environment.TickCount", EmitDefaultValue = false)]
		public int SystemEnvironmentTickCount { get; set; }

		[DataMember(Name = "System.Environment.UserName", EmitDefaultValue = false)]
		public string SystemEnvironmentUserName { get; set; }

		[DataMember(Name = "System.Environment.Version", EmitDefaultValue = false)]
		public string SystemEnvironmentVersion { get; set; }

		[DataMember(Name = "System.Environment.WorkingSet", EmitDefaultValue = false)]
		public long SystemEnvironmentWorkingSet { get; set; }
	}
}
