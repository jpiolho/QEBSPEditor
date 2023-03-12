using Radzen.Blazor;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace QEBSPEditor.Core.EntParsing;

public static class EntTools
{
    public static string Prettify(string code)
    {
        /*
        var sb = new StringBuilder();

        var tokens = CSLYEntParser.Tokenize(code);

        bool isValue = false;
        bool insideEntity = false;
        foreach(var token in tokens)
        {
            if(token.TokenID == CSLYEntParser.EntitiesTokens.OPEN_BRACKET)
            {
                sb.AppendLine("{");
                insideEntity = true;
            }
            else if(token.TokenID == CSLYEntParser.EntitiesTokens.STRING)
            {
                if(!isValue)
                {
                    sb.Append($"  {token.Value}");
                    isValue = true;
                }
                else
                {
                    sb.AppendLine($" {token.Value}");
                    isValue = false;
                }
            }
            else if(token.TokenID == CSLYEntParser.EntitiesTokens.COMMENT)
            {
                if (insideEntity)
                    sb.Append("  ");

                sb.Append(token.Value);
            }
            else if(token.TokenID == CSLYEntParser.EntitiesTokens.CLOSE_BRACKET)
            {
                insideEntity = false;
                sb.AppendLine("}");
            }
        }


        return sb.ToString();
        */

        var sb = new StringBuilder();

        var parser = new QuickEntParser(code);

        bool inEntity = false;

        parser.Start();
        while(parser.Next(out var token, out var _))
        {
            switch(token.Type)
            {
                case TokenType.InlineComment:
                    if (inEntity)
                        sb.Append("  ");
                    sb.AppendLine($"// {token.Value?.Trim()}");
                    break;
                case TokenType.OpenKey: sb.Append("  \""); break;
                case TokenType.CloseKey: sb.Append("\""); break;
                case TokenType.Key: sb.Append(token.Value); break;
                case TokenType.Value: sb.Append(token.Value); break;
                case TokenType.OpenValue: sb.Append(" \""); break;
                case TokenType.CloseValue: sb.AppendLine("\""); break;
                case TokenType.OpenEntity: inEntity = true; sb.AppendLine("{"); break;
                case TokenType.CloseEntity: inEntity = false; sb.AppendLine("}"); break;
            }
        }
      
        return sb.ToString();
    }

    public static string Minify(string code)
    {
        var sb = new StringBuilder();

        var parser = new QuickEntParser(code);
        parser.Start();
        while(parser.Next(out var token, out var _))
        {
            switch (token.Type)
            {
                case TokenType.InlineComment:
                    sb.AppendLine($"// {token.Value?.Trim()}"); break;
                case TokenType.OpenEntity:
                    sb.Append("{"); break;
                case TokenType.CloseEntity:
                    sb.Append("}"); break;

                case TokenType.OpenValue:
                case TokenType.CloseValue:
                case TokenType.OpenKey:
                case TokenType.CloseKey:
                    sb.Append("\""); break;

                case TokenType.Value:
                case TokenType.Key:
                    sb.Append(token.Value); break;
            }
        }

        return sb.ToString();
    }

    public static bool Validate(string code)
    {

        return true;
    }
}
