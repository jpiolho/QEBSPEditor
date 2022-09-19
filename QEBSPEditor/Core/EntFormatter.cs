using Radzen.Blazor;
using System.Text;

namespace QEBSPEditor.Core;

public static class EntFormatter
{
    public static string Prettify(string code)
    {
        var sb = new StringBuilder();


        bool inEntity = false;
        foreach (var parse in EntParser.Parse(code))
        {
            switch (parse.token)
            {
                case EntParser.Token.InlineComment:
                    if (inEntity)
                        sb.Append("  ");
                    sb.AppendLine($"// {parse.value.Trim()}");
                    break;
                case EntParser.Token.OpenEntity: inEntity = true; sb.AppendLine("{"); break;
                case EntParser.Token.OpenKey: sb.Append("  \""); break;
                case EntParser.Token.Key: sb.Append(parse.value); break;
                case EntParser.Token.CloseKey: sb.Append("\""); break;
                case EntParser.Token.OpenValue: sb.Append(" \""); break;
                case EntParser.Token.Value: sb.Append(parse.value); break;
                case EntParser.Token.CloseValue: sb.AppendLine("\""); break;
                case EntParser.Token.CloseEntity: inEntity = false; sb.AppendLine("}"); break;
            }

        }

        return sb.ToString();
    }

    public static string Minify(string code)
    {
        var sb = new StringBuilder();

        foreach (var parse in EntParser.Parse(code))
        {
            switch (parse.token)
            {
                case EntParser.Token.InlineComment:
                    sb.AppendLine($"// {parse.value.Trim()}"); break;
                case EntParser.Token.OpenEntity:
                    sb.Append("{"); break;
                case EntParser.Token.CloseEntity:
                    sb.Append("}"); break;

                case EntParser.Token.OpenValue:
                case EntParser.Token.CloseValue:
                case EntParser.Token.OpenKey:
                case EntParser.Token.CloseKey:
                    sb.Append("\""); break;

                case EntParser.Token.Value:
                case EntParser.Token.Key:
                    sb.Append(parse.value); break;
            }
        }

        return sb.ToString();
    }
}
