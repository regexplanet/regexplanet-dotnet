using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebUtility;

public record TestOutput(Boolean success, string message, string html);

public record TestInput(string regex, string replacement, string[] options, string[] inputs);

public record SerializableCapture(int Index, int Length, string Value);
public record SerializableGroup(string Name, bool Success, SerializableCapture[] Captures);
public record SerializableMatch(int Index, int Length, bool Success, string Value, SerializableGroup[] Groups);


public static class TestRunner
{
    public static TestOutput RunTest(TestInput testInput)
    {

        if (String.IsNullOrEmpty(testInput.regex))
        {
            return new TestOutput(false, "No regular expression to test!", "");
        }

        TimeSpan timeout = TimeSpan.FromSeconds(2.5);
        RegexOptions options = GetOptions(testInput.options);
        // TODO handle invalid combinations of options

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<table class=\"table table-bordered table-striped bordered-table zebra-striped\" style=\"width:auto;\">");
        sb.AppendLine("\t<tbody>");

        sb.AppendLine("\t\t<tr>");
        sb.AppendLine("\t\t\t<td>Regular Expression</td>");
        sb.Append("\t\t\t<td>");
        sb.Append(HtmlEncode(testInput.regex));
        sb.Append("</td>");
        sb.AppendLine("\t\t</tr>");

        sb.AppendLine("\t\t<tr>");
        sb.AppendLine("\t\t\t<td>as a .Net string</td>");
        sb.Append("\t\t\t<td>&quot;");
        char[] chars = testInput.regex.ToCharArray();
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
                sb.Append(HtmlEncode(Char.ToString(character)));
            }
        }
        sb.Append("&quot;</td>");
        sb.AppendLine("\t\t</tr>");

        sb.AppendLine("\t\t<tr>");
        sb.AppendLine("\t\t\t<td>Replacement</td>");
        sb.Append("\t\t\t<td>");
        sb.Append(HtmlEncode(testInput.replacement));
        sb.Append("</td>");
        sb.AppendLine("\t\t</tr>");

        Regex? regularExpression = null;
        try
        {
            regularExpression = new Regex(testInput.regex, options);
        }
        catch (Exception en)
        {
            sb.AppendLine("\t</tbody>");
            sb.AppendLine("</table>");
            return new TestOutput(false, en.Message, sb.ToString());
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

        if (testInput.inputs == null || testInput.inputs.Length == 0)
        {
            return new TestOutput(true, "", sb.ToString());
        }

        if (regularExpression != null)
        {
            sb.AppendLine("<table class=\"table table-bordered table-striped bordered-table zebra-striped\">");
            sb.AppendLine("\t<thead>");
            sb.AppendLine("\t\t<tr>");
            sb.AppendLine("\t\t\t<th style=\"text-align:center;\">Test</th>");
            sb.AppendLine("\t\t\t<th>Target String</th>");
            sb.AppendLine("\t\t\t<th>Count()</th>");
            sb.AppendLine("\t\t\t<th>IsMatch()</th>");
            sb.AppendLine("\t\t\t<th>Replace()</th>");
            sb.AppendLine("\t\t\t<th>Split()</th>");
            sb.AppendLine("\t\t\t<th>Matches()</th>");
            sb.AppendLine("\t\t</tr>");
            sb.AppendLine("\t</thead>");

            sb.AppendLine("\t<tbody>");

            for (int loop = 0, len = testInput.inputs.Length; loop < len; loop++)
            {
                String test = testInput.inputs[loop];
                if (test == null || test.Length == 0)
                {
                    continue;
                }
                sb.AppendLine("\t\t<tr>");
                sb.Append("\t\t\t<td style=\"text-align:center\">");
                sb.Append(loop + 1);
                sb.AppendLine("</td>");

                sb.Append("\t\t\t<td>");
                sb.Append(HtmlEncode(test));
                sb.AppendLine("</td>");

                sb.Append("\t\t\t<td style=\"text-align:center;\">");
                try
                {
                    var findCount = Regex.Count(test, testInput.regex, options, timeout);
                    sb.Append(findCount);
                }
                catch (Exception ex)
                {
                    sb.Append("<i>");
                    sb.Append(HtmlEncode(ex.Message));
                    sb.Append("</i>");
                }
                sb.AppendLine("</td>");

                sb.Append("\t\t\t<td>");
                try
                {
                    var isMatch = Regex.IsMatch(test, testInput.regex, options, timeout);
                    sb.Append(isMatch);
                }
                catch (Exception ex)
                {
                    sb.Append("<i>");
                    sb.Append(HtmlEncode(ex.Message));
                    sb.Append("</i>");
                }
                sb.AppendLine("</td>");

                sb.Append("\t\t\t<td>");
                try
                {
                    var replaced = Regex.Replace(test, testInput.regex, testInput.replacement, options, timeout);
                    sb.Append(HtmlEncode(replaced));
                }
                catch (Exception ex)
                {
                    sb.Append("<i>");
                    sb.Append(HtmlEncode(ex.Message));
                    sb.Append("</i>");
                }
                sb.AppendLine("</td>");

                sb.Append("\t\t\t<td>");
                try
                {
                    var splits = Regex.Split(test, testInput.regex, options, timeout);
                    sb.Append("<code>");
                    sb.Append(HtmlEncode(JsonSerializer.Serialize(splits)));
                    sb.Append("</code>");
                }
                catch (Exception ex)
                {
                    sb.Append("<i>");
                    sb.Append(HtmlEncode(ex.Message));
                    sb.Append("</i>");
                }
                sb.AppendLine("</td>");

                sb.Append("\t\t\t<td>");
                try
                {
                    var ms = Regex.Matches(test, testInput.regex, options, timeout);
                    if (ms.Count == 0) {
                        sb.Append("<i>Empty</i>");
                    } else {
                        for (int matchIndex = 0; matchIndex < ms.Count; matchIndex++)
                        {
                            var m = ms[matchIndex];
                            sb.Append($"[{matchIndex}]: <code>");
                            sb.Append(HtmlEncode(MatchToString(m)));
                            sb.AppendLine("</code><br/>");
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine("\t\t\t<td>ERROR: ");
                    sb.Append(HtmlEncode(ex.Message));
                    sb.AppendLine("</td>");
                }
                sb.AppendLine("</td>");
                sb.AppendLine("\t\t</tr>");
            }
            sb.AppendLine("\t</tbody>");
            sb.AppendLine("</table>");
        }

        var testOutput = new TestOutput(true, "", sb.ToString());

        return testOutput;
    }

    static public RegexOptions GetOptions(string[] optionsArray)
    {
        RegexOptions options = RegexOptions.None;
        if (optionsArray == null)
        {
            return options;
        }

        foreach (string optionString in optionsArray)
        {
            switch (optionString)
            {
                case "ignorecase":
                    options |= RegexOptions.IgnoreCase;
                    break;
                case "locale":
                    options |= RegexOptions.CultureInvariant;
                    break;
                case "multiline":
                    options |= RegexOptions.Multiline;
                    break;
                case "dotall":
                    options |= RegexOptions.Singleline;
                    break;
                case "explicitcapture":
                    options |= RegexOptions.ExplicitCapture;
                    break;
                case "comment":
                    options |= RegexOptions.IgnorePatternWhitespace;
                    break;
                case "righttoleft":
                    options |= RegexOptions.RightToLeft;
                    break;
                case "javascript":
                    options |= RegexOptions.ECMAScript;
                    break;
            }
        }
        return options;
    }

    static public String BooleanToString(bool b)
    {
        return b ? "Yes" : "No";
    }

    static public String MatchToString(Match m)
    {
        SerializableGroup[] sgroups = new SerializableGroup[m.Groups.Count];
        for (int groupIndex = 0; groupIndex < m.Groups.Count; groupIndex++)
        {
            Group g = m.Groups[groupIndex];
            SerializableCapture[] scap = new SerializableCapture[g.Captures.Count];
            for (int i = 0; i < g.Captures.Count; i++)
            {
                Capture c = g.Captures[i];
                scap[i] = new SerializableCapture(c.Index, c.Length, c.Value);
            }
            sgroups[groupIndex++] = new SerializableGroup(g.Name, g.Success, scap);
        }
        Console.WriteLine(sgroups);
        Console.WriteLine($"m.Groups.Count: {m.Groups.Count}");
        //sgroups.Append(new SerializableGroup("test", false, []));

        var sm = new SerializableMatch(m.Index, m.Length, m.Success, m.Value, sgroups);
        return JsonSerializer.Serialize(sm);
    }

}