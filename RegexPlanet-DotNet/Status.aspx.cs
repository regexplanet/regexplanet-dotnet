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
			statusResult.Test = "It worked!";

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
