using QEBSPEditor.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace QEBSPEditor.Core.EntParsing;

public class QuickEntParser : IEntParser
{
    private enum Modes
    {
        Root,
        Entity,
        Key,
        Interkey,
        Value,
        Comment
    };

    private string _code;
    private IEnumerator<ParseToken> _parser;


    public QuickEntParser(string code)
    {
        _code = code;
    }

    public void Start()
    {
        _parser = Parse(_code).GetEnumerator();
    }

    public bool Next([NotNullWhen(true)] out ParseToken? token, out ParseError? error)
    {
        if (_parser.MoveNext())
        {
            token = _parser.Current;
            error = null;
            return true;
        }

        token = null;
        error = null;
        return false;
    }

    public bool NextEntity([NotNullWhen(true)] out Entity? entity, out ParseError? error)
    {
        entity = null;

        string? keyBuffer = null;
        while(Next(out var token, out error))
        {
            if (error != null)
                return false;

            switch (token.Type)
            {
                case TokenType.OpenEntity:
                    entity = new Entity()
                    {
                        SourceHint = new()
                        {
                            OffsetStart = token.Offset
                        }
                    };
                    break;
                case TokenType.CloseEntity:
                    entity!.SourceHint!.OffsetEnd = token.Offset + 1;
                    return true;

                case TokenType.Key:
                    keyBuffer = token.Value;
                    break;
                case TokenType.Value:
                    entity!.SetKeyValue(keyBuffer!, token.Value!);
                    keyBuffer = null;
                    break;
            }
        }


        return false;
    }

    public static List<Entity> ParseEntities(string code)
    {
        var parser = new QuickEntParser(code);
        parser.Start();

        var entities = new List<Entity>();

        while(parser.NextEntity(out var entity, out _))
            entities.Add(entity);

        return entities;
    }

    private IEnumerable<ParseToken> Parse(string code)
    {

        var buffer = new StringBuilder();
        bool escape = false;

        var position = new ParsePosition();
        position.Line = 1;
        position.Column = 1;

        var stack = new Stack<Modes>();
        stack.Push(Modes.Root);

        for (var i = 0; i < code.Length; i++)
        {
            position.Offset = i;
            var c = code[i];

            position.Column++;
            if (c == '\n')
            {
                position.Line++;
                position.Column = 1;
            }

            var mode = stack.Peek();

            if (mode == Modes.Root || mode == Modes.Interkey || mode == Modes.Entity)
            {
                if (c == '/' && i < code.Length - 1 && code[i + 1] == '/')
                {
                    i++;
                    buffer.Clear();
                    stack.Push(Modes.Comment);
                    continue;
                }
            }

            if (mode == Modes.Comment)
            {
                if (c == '\r' || c == '\n')
                {
                    stack.Pop();
                    yield return ParseToken.Create(TokenType.InlineComment, position, buffer);
                }

                buffer.Append(c);
            }
            else if (mode == Modes.Root)
            {
                if (c == '{')
                {
                    yield return ParseToken.Create(TokenType.OpenEntity, position, "{");
                    stack.Push(Modes.Entity);
                }
            }
            else if (mode == Modes.Entity)
            {
                if (c == '"')
                {
                    buffer.Clear();
                    yield return ParseToken.Create(TokenType.OpenKey, position, "\"");
                    stack.Push(Modes.Key);
                }
                else if (c == '}')
                {
                    yield return ParseToken.Create(TokenType.CloseEntity, position, "}");
                    stack.Pop();
                }
            }
            else if (mode == Modes.Key || mode == Modes.Value)
            {
                buffer.Append(c);

                if (c == '\\')
                    escape = true;
                else if (c == '"')
                {
                    if (!escape)
                    {
                        buffer.Length--;

                        if (mode == Modes.Key)
                        {
                            yield return ParseToken.Create(TokenType.Key, position, buffer);
                            yield return ParseToken.Create(TokenType.CloseKey, position, "\"");

                            buffer.Clear();
                            stack.Pop();
                            stack.Push(Modes.Interkey);
                        }
                        else if (mode == Modes.Value)
                        {
                            yield return ParseToken.Create(TokenType.Value, position, buffer);
                            yield return ParseToken.Create(TokenType.CloseValue, position, "\"");

                            buffer.Clear();
                            stack.Pop();
                        }
                    }
                    escape = false;
                }
                else
                {
                    escape = false;
                }
            }
            else if (mode == Modes.Interkey)
            {
                if (c == '"')
                {
                    buffer.Clear();
                    yield return ParseToken.Create(TokenType.OpenValue, position, "\"");

                    stack.Pop();
                    stack.Push(Modes.Value);
                }
            }
        }
    }

}
