namespace QEBSPEditor.Core.EntParsing;

public enum TokenType
{
    Unknown,
    Whitespace,

    OpenEntity,
    CloseEntity,

    OpenKey,
    Key,
    CloseKey,

    OpenValue,
    Value,
    CloseValue,

    InlineComment,
}