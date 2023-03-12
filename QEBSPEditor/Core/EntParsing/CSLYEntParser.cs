using sly.lexer;
using sly.parser.generator;
using sly.parser.parser;

namespace QEBSPEditor.Core.EntParsing
{
    public class CSLYEntParser
    {
        public class Entity
        {
            public List<KeyValuePair<string,string>> KeyValues { get; set; } = new();
        }

        public enum EntitiesTokens
        {
            [Lexeme("([\"'])((\\\\{2})*|(.*?[^\\\\](\\\\{2})*))\\1")]
            STRING,

            [Lexeme("\\{")]
            OPEN_BRACKET,

            [Lexeme("\\}")]
            CLOSE_BRACKET,

            [Lexeme("//.*\n")]
            COMMENT,

            
            [Lexeme("[ \\t]+", isSkippable: true)]
            WHITESPACE,

            [Lexeme("[\\r\\n]+", isSkippable: true, isLineEnding: true)]
            NEW_LINE
        }

        private class EntitiesParser
        {
            [Production("keyvalue: STRING STRING")]
            public KeyValuePair<string,string> KeyValue(Token<EntitiesTokens> key, Token<EntitiesTokens> value)
            {
                return new KeyValuePair<string, string>(key.Value.Trim('"').Trim(), value.Value.Trim('"'));
            }

            [Production("entity: OPEN_BRACKET[d] keyvalue* CLOSE_BRACKET[d]")]
            public Entity Entity(List<object> keyvalues)
            {
                return new Entity()
                {
                    KeyValues = keyvalues.Cast<KeyValuePair<string,string>>().ToList()
                };
            }

            [Production("entities: entity*")]
            public List<Entity> Entities(List<object> entities)
            {
                return entities.Cast<Entity>().ToList();
            }
        }

        public static List<Entity> Parse(string content)
        {
            var parserInstance = new EntitiesParser();
            var builder = new ParserBuilder<EntitiesTokens, object>();
            var parser = builder.BuildParser(parserInstance, ParserType.EBNF_LL_RECURSIVE_DESCENT, "entities").Result;

            var result = parser.Parse(content);

            if (result.IsError)
                throw new AggregateException(result.Errors.Select(err => new Exception($"Parsing error: {err}")));

            return (List<Entity>)result.Result;
        }

        public static TokenChannels<EntitiesTokens> Tokenize(string content)
        {
            var parserInstance = new EntitiesParser();
            var builder = new ParserBuilder<EntitiesTokens, object>();
            var parser = builder.BuildParser(parserInstance, ParserType.EBNF_LL_RECURSIVE_DESCENT, "entities").Result;
            
            var tokens = parser.Lexer.Tokenize(content);

            if (tokens.IsError)
                throw new Exception(tokens.Error.ToString());

            return tokens.Tokens;
        }
    }
}
