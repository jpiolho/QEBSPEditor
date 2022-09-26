using System.Diagnostics.CodeAnalysis;

namespace QEBSPEditor.Core.EntParsing
{
    public interface IEntParser
    {
        void Start();
        bool Next([NotNullWhen(true)] out ParseToken? token, out ParseError? error);
    }
}
