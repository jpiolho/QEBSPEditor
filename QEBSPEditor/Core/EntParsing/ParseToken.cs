using System.Text;

namespace QEBSPEditor.Core.EntParsing;

public class ParseToken
{
    public TokenType Type { get; init; }
    public string? Value { get; init; }
    public int Line { get; init; }
    public int Column { get; init; }
    public int Offset { get; init; }

    private ParseToken(TokenType type, ParsePosition position, string? value = null)
    {
        Type = type;
        Line = position.Line;
        Column = position.Column;
        Offset = position.Offset;
        Value = value;
    }


    public static ParseToken Create(TokenType type, ParsePosition position, StringBuilder? value = null)
    {
        return Create(type, position, value?.ToString() ?? null);
    }
    public static ParseToken Create(TokenType type, ParsePosition position, string? value = null)
    {
        return new ParseToken(type, position, value);
    }
}
