using QEBSPEditor.Core.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace QEBSPEditor.Core.EntParsing
{
    public class FancyEntParser : IEntParser
    {
        private static OnMatchHandler Lambda(OnMatchHandler h) => h;
        private class Mode
        {
            public List<(string, OnMatchHandler)> AllMatches { get; } = new List<(string, OnMatchHandler)>();

            public Mode Inherits(params Mode[] inherits) => Inherits(inherits.SelectMany(m => m.AllMatches));
            public Mode Inherits(params IEnumerable<(string, OnMatchHandler)>[] inherits)
            {
                foreach (var inherited in inherits)
                    this.AllMatches.AddRange(inherited);
                return this;
            }

            public Mode Matches(params (string, OnMatchHandler)[] matches)
            {
                this.AllMatches.AddRange(matches);
                return this;
            }
        }

        private string _code;
        private int _offset;

        private Queue<ParseToken> _tokens;
        private ParseError? _error;

        private RegexBucket _regex;
        private ParsePosition _position = new ParsePosition();
        private delegate void OnMatchHandler(Match match);

        private Stack<Mode> _mode = new Stack<Mode>();


        private Mode
            ModeWhitespace,
            ModeRoot,
            ModeEntity,
            ModeValue
        = null!;


        public FancyEntParser(string code)
        {
            this._code = code;
            _regex = RegexBucket.Default;


            #pragma warning disable IDE0055 // Disables auto code formatting
            
            ModeWhitespace = new Mode()
                .Matches(
                    ("[ \r\n\t]+", (m) => { ReturnToken(TokenType.Whitespace, m.Value); }),
                    ("//(.*)(?:[\\r\\n]|$)",(m) => { ReturnToken(TokenType.InlineComment, m.Groups[1].Value); })
                );

            ModeRoot = new Mode()
                .Inherits(ModeWhitespace)
                .Matches(
                    ("{", m => { _mode.Push(ModeEntity!); ReturnToken(TokenType.OpenEntity, m.Value); })
                );

            ModeEntity = new Mode()
                .Matches(
                    (@"""([^""\\]*(?:\\.[^""\\]*)*)""", m => { 
                        _mode.Push(ModeValue!);
                        ReturnToken(TokenType.OpenKey,"\"");
                        ReturnToken(TokenType.Key, m.Groups[1].Value);
                        ReturnToken(TokenType.CloseKey, "\"");
                    }),
                    ("}", m => { _mode.Pop(); ReturnToken(TokenType.CloseEntity, m.Value); })
                )
                .Inherits(ModeWhitespace);

            ModeValue = new Mode()
                .Inherits(ModeWhitespace)
                .Matches(
                    (@"""([^""\\]*(?:\\.[^""\\]*)*)""", m => { 
                        _mode.Pop();
                        ReturnToken(TokenType.OpenValue, "\"");
                        ReturnToken(TokenType.Value, m.Groups[1].Value);
                        ReturnToken(TokenType.CloseValue, "\"");
                    }),
                    ("}", m => { _mode.PopUntil(ModeRoot); Error("Expected value, but instead entity got closed"); ReturnToken(TokenType.CloseEntity, m.Value); })
                );

            #pragma warning restore IDE0055


            _mode.Push(ModeRoot);
        }


        public void Start()
        {
            _tokens = new Queue<ParseToken>();

            _mode.Clear();
            _mode.Push(ModeRoot);

            _position.Offset = 0;
            _position.Line = 1;
            _position.Column = 1;
        }


        public bool Next([NotNullWhen(true)] out ParseToken? token, out ParseError? error)
        {
            error = null;
            token = null;

            if (_tokens.Count > 0)
            {
                token = _tokens.Dequeue();
                return true;
            }

            _error = null;

            foreach (var match in _mode.Peek().AllMatches)
            {
                if (_offset == _code.Length)
                    return false;

                var m = _regex.Match(_code, "\\G" + match.Item1, _offset);

                if (m.Success)
                {
                    match.Item2(m);
                    token = _tokens.Dequeue();
                    error = _error;
                    _offset += m.Length;
                    UpdatePosition(m);


                    return true;
                }
            }

            var unknown = _regex.Match("\\G(.+?)[\\r\\n\\s{}]", _offset);

            if (unknown.Success)
            {
                var unknownValue = unknown.Captures[0].Value;
                error = new ParseError() { Error = $"Unknown identifier: {unknownValue}" };
                token = ReturnToken(TokenType.Unknown, unknownValue);
                _offset += unknown.Length;
                UpdatePosition(unknown);

                return true;
            }

            return false;
        }


        private void UpdatePosition(Match lastMatch)
        {
            var span = lastMatch.ValueSpan;
            for (var i = 0; i < span.Length; i++)
            {
                _position.Offset++;

                if (span[i] == '\n')
                {
                    _position.Line++;
                    _position.Column = 1;
                }
                else
                {
                    _position.Column++;
                }
            }
        }



        private void Error(string message)
        {
            _error = new ParseError() { Position = _position, Error = message };
        }


        private ParseToken ReturnToken(TokenType type, string? value)
        {
            var t = ParseToken.Create(type, _position, value);
            _tokens.Enqueue(t);
            return t;
        }
    }
}
