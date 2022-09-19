using System.Text;

namespace QEBSPEditor.Core
{
    public class EntParser
    {
        public enum Token
        {
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



        public static IEnumerable<(Token token,string value)> Parse(string code)
        {
            const int MODE_ROOT = 0;
            const int MODE_ENTITY = 1;
            const int MODE_KEY = 2;
            const int MODE_INTERKEY = 3;
            const int MODE_VALUE = 4;
            const int MODE_COMMENT = 5;

            var buffer = new StringBuilder();
            bool escape = false;

            var stack = new Stack<int>();
            stack.Push(MODE_ROOT);

            for (var i = 0; i < code.Length; i++)
            {
                var c = code[i];


                var mode = stack.Peek();
                
                if(mode == MODE_ROOT || mode == MODE_INTERKEY || mode == MODE_ENTITY)
                {
                    if(c == '/' && (i < code.Length - 1 && code[i+1] == '/'))
                    {
                        i++;
                        buffer.Clear();
                        stack.Push(MODE_COMMENT);
                        continue;
                    }
                }
                
                if(mode == MODE_COMMENT)
                {
                    if(c == '\r' || c == '\n')
                    {
                        stack.Pop();
                        yield return (Token.InlineComment, buffer.ToString());
                    }

                    buffer.Append(c);
                }
                else if (mode == MODE_ROOT)
                {
                    if (c == '{')
                    {
                        yield return (Token.OpenEntity, "{");
                        stack.Push(MODE_ENTITY);
                    }
                }
                else if (mode == MODE_ENTITY)
                {
                    if (c == '"')
                    {
                        buffer.Clear();
                        yield return (Token.OpenKey, "\"");
                        stack.Push(MODE_KEY);
                    }
                    else if (c == '}')
                    {
                        yield return (Token.CloseEntity, "}");
                        stack.Pop();
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
                                stack.Pop();
                                stack.Push(MODE_INTERKEY);
                            }
                            else if (mode == MODE_VALUE)
                            {
                                yield return (Token.Key, buffer.ToString());
                                yield return (Token.CloseValue, "\"");

                                buffer.Clear();
                                stack.Pop();
                            }
                        }
                        escape = false;
                    }
                }
                else if (mode == MODE_INTERKEY)
                {
                    if (c == '"')
                    {
                        buffer.Clear();
                        yield return (Token.OpenValue, "\"");

                        stack.Pop();
                        stack.Push(MODE_VALUE);
                    }
                }
            }
        }
    }
}
