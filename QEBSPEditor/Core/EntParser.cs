using System.Text;

namespace QEBSPEditor.Core
{
    public class EntParser
    {
        public enum Token
        {
            OpenEntity,
            
            OpenKey,
            Key,
            CloseKey,

            OpenValue,
            Value,
            CloseValue,

            CloseEntity
        }



        public static IEnumerable<(Token token,string value)> Parse(string code)
        {
            const int MODE_ROOT = 0;
            const int MODE_ENTITY = 1;
            const int MODE_KEY = 2;
            const int MODE_INTERKEY = 3;
            const int MODE_VALUE = 4;

            var buffer = new StringBuilder();
            int mode = 0;
            bool escape = false;
            for (var i = 0; i < code.Length; i++)
            {
                var c = code[i];

                if (mode == MODE_ROOT)
                {
                    if (c == '{')
                    {
                        yield return (Token.OpenEntity, "{");
                        mode = MODE_ENTITY;
                    }
                }
                else if (mode == MODE_ENTITY)
                {
                    if (c == '"')
                    {
                        yield return (Token.OpenKey, "\"");
                        mode = MODE_KEY;
                    }
                    else if (c == '}')
                    {
                        yield return (Token.CloseEntity, "}");
                        mode = MODE_ROOT;
                    }
                }
                else if (mode == MODE_KEY || mode == MODE_VALUE)
                {
                    buffer.Append(c);

                    if (c == '\\')
                        escape = true;
                    else if (c == '"')
                    {
                        if (!escape)
                        {
                            buffer.Length--;

                            if (mode == MODE_KEY)
                            {
                                yield return (Token.Key, buffer.ToString());
                                yield return (Token.CloseKey, "\"");
                                
                                buffer.Clear();
                                mode = MODE_INTERKEY;
                            }
                            else if (mode == MODE_VALUE)
                            {
                                yield return (Token.Key, buffer.ToString());
                                yield return (Token.CloseValue, "\"");

                                buffer.Clear();
                                mode = MODE_ENTITY;
                            }
                        }
                        escape = false;
                    }
                }
                else if (mode == MODE_INTERKEY)
                {
                    if (c == '"')
                    {
                        yield return (Token.OpenValue, "\"");
                        mode = MODE_VALUE;
                    }
                }
            }
        }
    }
}
