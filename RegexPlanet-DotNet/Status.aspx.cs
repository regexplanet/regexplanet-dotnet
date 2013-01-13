using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexPlanet_DotNet
{
	public partial class Status : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			StatusResult statusResult = new StatusResult();
			statusResult.Success = true;
			statusResult.Version = System.Environment.Version.ToString();
			statusResult.SystemEnvironmentCommandLine = System.Environment.CommandLine;
			statusResult.SystemEnvironmentCurrentDirectory = System.Environment.CurrentDirectory;
			statusResult.SystemEnvironmentIs64BitOperatingSystem = System.Environment.Is64BitOperatingSystem;
			statusResult.SystemEnvironmentIs64BitProcess = System.Environment.Is64BitProcess;
			statusResult.SystemEnvironmentMachineName = System.Environment.MachineName;
			statusResult.SystemEnvironmentOSVersion = System.Environment.OSVersion.ToString();
			statusResult.SystemEnvironmentProcessorCount = System.Environment.ProcessorCount;
			statusResult.SystemEnvironmentSystemDirectory = System.Environment.SystemDirectory;
			statusResult.SystemEnvironmentSystemPageSize = System.Environment.SystemPageSize;
			statusResult.SystemEnvironmentTickCount = System.Environment.TickCount;
			statusResult.SystemEnvironmentUserName = System.Environment.UserName;
			statusResult.SystemEnvironmentVersion = System.Environment.Version.ToString();
			statusResult.SystemEnvironmentWorkingSet = System.Environment.WorkingSet;

			String result = Serialize<StatusResult>(statusResult);
			string callback = Request.Params["callback"];
			if (!String.IsNullOrEmpty(callback))
			{
				Response.Write(callback);
				Response.Write("(");
				Response.Write(result);
				Response.Write(")");
			}
			else
			{
				Response.AppendHeader("Access-Control-Allow-Origin", "*");
				Response.AppendHeader("Access-Control-Allow-Methods", "POST, GET");
				Response.AppendHeader("Access-Control-Max-Age", "604800");
				Response.Write(result);
			}
		}

		private string Serialize<T>(T obj)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
			using (MemoryStream ms = new MemoryStream())
			{
				serializer.WriteObject(ms, obj);
				return Encoding.Default.GetString(ms.ToArray());
			}
		}
	}
}
