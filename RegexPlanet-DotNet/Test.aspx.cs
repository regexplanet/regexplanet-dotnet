using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexPlanet_DotNet
{
	public partial class Test : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			TestResult testResult = new TestResult();
			string regEx = Request.Params["regex"];
			string callback = Request.Params["callback"];
			string replacement = Request.Params["replacement"];
			string[] inputs = Request.Params.GetValues("input");

			TimeSpan timeout = TimeSpan.FromSeconds(10);

			if (String.IsNullOrEmpty(regEx))
			{
				testResult.Success = false;
				testResult.Message = "No regex to test";
			}
			else
			{
				RegexOptions options = GetOptions();
				// TODO handle invalid combinations of options

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("<table class=\"table table-bordered table-striped bordered-table zebra-striped\" style=\"width:auto;\">");
				sb.AppendLine("\t<tbody>");

				sb.AppendLine("\t\t<tr>");
				sb.AppendLine("\t\t\t<td>Regular Expression</td>");
				sb.Append("\t\t\t<td>");
				sb.Append(Server.HtmlEncode(regEx));
				sb.Append("</td>");
				sb.AppendLine("\t\t</tr>");

				sb.AppendLine("\t\t<tr>");
				sb.AppendLine("\t\t\t<td>as a .Net string</td>");
				sb.Append("\t\t\t<td>&quot;");
				char[] chars = regEx.ToCharArray();
				foreach (Char character in chars)
				{
					if (character == '"')
					{
						sb.Append("\\&quot;");
					}
					else if (character == '\\')
					{
						sb.Append("\\\\");
					}
					else
					{
						sb.Append(Server.HtmlEncode(Char.ToString(character)));
					}
				}
				sb.Append("&quot;</td>");
				sb.AppendLine("\t\t</tr>");

				sb.AppendLine("\t\t<tr>");
				sb.AppendLine("\t\t\t<td>Replacement</td>");
				sb.Append("\t\t\t<td>");
				sb.Append(Server.HtmlEncode(replacement));
				sb.Append("</td>");
				sb.AppendLine("\t\t</tr>");

				Regex regularExpression = null;
				try
				{
					regularExpression = new Regex(regEx, options);
				}
				catch (Exception en)
				{
					sb.AppendLine("\t\t<tr>");
					sb.AppendLine("\t\t\t<td>Error</td>");
					sb.Append("\t\t\t<td>");
					sb.Append(Server.HtmlEncode(en.Message));
					sb.Append("</td>");
					sb.AppendLine("\t\t</tr>");
				}

				int groupCount = 0;
				if (regularExpression != null)
				{
					sb.AppendLine("\t\t<tr>");
					sb.AppendLine("\t\t\t<td>GetGroupNames()</td>");
					sb.Append("\t\t\t<td>");
					sb.Append(string.Join(",", regularExpression.GetGroupNames()));
					sb.Append("</td>");
					sb.AppendLine("\t\t</tr>");
					sb.AppendLine("\t\t<tr>");

					int[] groupNumbers = regularExpression.GetGroupNumbers();
					sb.AppendLine("\t\t\t<td>GetGroupNumbers()</td>");
					sb.Append("\t\t\t<td>");
					sb.Append(string.Join(",", groupNumbers));
					sb.Append("</td>");
					sb.AppendLine("\t\t</tr>");
					groupCount = groupNumbers.Length;
				}
				sb.AppendLine("\t</tbody>");
				sb.AppendLine("</table>");

				if (regularExpression != null)
				{
					sb.AppendLine("<table class=\"table table-bordered table-striped bordered-table zebra-striped\">");
					sb.AppendLine("\t<thead>");
					sb.AppendLine("\t\t<tr>");
					sb.AppendLine("\t\t\t<th style=\"text-align:center;\">Test</th>");
					sb.AppendLine("\t\t\t<th>Target String</th>");
					sb.AppendLine("\t\t\t<th>Match()</th>");
					sb.AppendLine("\t\t\t<th>Result()</th>");
					for (int loop = 0; loop < groupCount; loop++)
					{
						sb.Append("\t\t\t<th>Groups[");
						sb.Append(loop);
						sb.AppendLine("]</th>");
					}
					sb.AppendLine("\t\t</tr>");
					sb.AppendLine("\t</thead>");

					sb.AppendLine("\t<tbody>");

					for (int loop = 0, len = inputs == null ? 0 : inputs.Length; loop < len; loop++)
					{
						String test = inputs[loop];
						if (test == null || test.Length == 0)
						{
							continue;
						}
						sb.AppendLine("\t\t<tr>");
						sb.Append("\t\t\t<td style=\"text-align:center\">");
						sb.Append(loop + 1);
						sb.AppendLine("</td>");

						sb.Append("\t\t\t<td>");
						sb.Append(Server.HtmlEncode(test));
						sb.AppendLine("</td>");

						Match m = null;
						try
						{
							m = Regex.Match(test, regEx, options, timeout);
						}
						catch (RegexMatchTimeoutException rmte)
						{
							sb.AppendLine("\t\t\t<td colspan=\"3\">ERROR: RegexMatchTimeoutException</td>");
							sb.AppendLine("\t\t</tr>");
							continue;
						}

						sb.Append("\t\t\t<td>");
						sb.Append(Server.HtmlEncode(BooleanToString(m.Success)));
						sb.AppendLine("</td>");

						sb.Append("\t\t\t<td>");
						try
						{
							if (m.Success)
							{
								sb.Append(Server.HtmlEncode(m.Result(replacement)));
							}
							else
							{
								sb.Append("&nbsp;");
							}
						}
						catch (Exception ex)
						{
							sb.Append("<i>(ERROR: ");
							sb.Append(Server.HtmlEncode(ex.Message));
							sb.Append(")</i>");
						}
						sb.AppendLine("</td>");

						int count = 0;
						bool ifFirst = true;
						while (m.Success)
						{
							count = 0;
							if (ifFirst == true)
							{
								ifFirst = false;
							}
							else
							{
								sb.AppendLine("\t\t</tr>");
								sb.AppendLine("\t\t<tr>");
								sb.AppendLine("\t\t\t<td colspan=\"4\" style=\"text-align:right\">NextMatch()</td>");
							}

							for (int group = 0; group < m.Groups.Count; group++)
							{
								count++;
								sb.Append("\t\t\t<td>");
								sb.Append(Server.HtmlEncode(m.Groups[group].Value));
								sb.AppendLine("</td>");
							}
							for (; count < groupCount; count++)
							{
								// for group 0
								sb.AppendLine("\t\t\t<td>&nbsp;</td>");
							}
							m = m.NextMatch();
						}
						for (; count < groupCount; count++)
						{
							// for group 0
							sb.AppendLine("\t\t\t<td>&nbsp;</td>");
						}

						sb.AppendLine("\t\t</tr>");
					}
					sb.AppendLine("\t</tbody>");
					sb.AppendLine("</table>");

					testResult.Success = true;
					testResult.Html = sb.ToString();
				}
			}

			String result = Serialize<TestResult>(testResult);
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

		private RegexOptions GetOptions()
		{
			RegexOptions options = RegexOptions.None;
			if (Request.Params["option"] == null)
			{
				return options;
			}

			string[] optionsArray = Request.Params.GetValues("option");
			foreach (string optionString in optionsArray)
			{
				switch (optionString)
				{
					case "ignorecase":
						options |= RegexOptions.IgnoreCase;
						break;
					case "locale":
						options |=RegexOptions.CultureInvariant;
						break;
					case "multiline":
						options |=RegexOptions.Multiline;
						break;
					case "dotall":
						options |=RegexOptions.Singleline;
						break;
					case "explicitcapture":
						options |=RegexOptions.ExplicitCapture;
						break;
					case "comment":
						options |=RegexOptions.IgnorePatternWhitespace;
						break;
					case "righttoleft":
						options |=RegexOptions.RightToLeft;
						break;
					case "javascript":
						options |=RegexOptions.ECMAScript;
						break;
				}
			}
			return options;
		}

		private String BooleanToString(bool b)
		{
			return b ? "Yes" : "No";
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