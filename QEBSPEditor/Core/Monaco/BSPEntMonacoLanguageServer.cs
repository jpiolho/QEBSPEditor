using Antlr4.Runtime;
using QEBSPEditor.Components;
using System.Diagnostics.Metrics;

namespace QEBSPEditor.Core.Monaco;

public class BSPEntMonacoLanguageServer : MonacoLanguageServer
{
    private class ErrorListener : IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
    {
        private const int SeverityError = 8;

        public List<Marker> Errors { get; set; } = new List<Marker>();
        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            if (Errors.Count > 10)
                return;

            Errors.Add(new Marker()
            {
                StartLineNumber = line,
                EndLineNumber = line,
                StartColumn = charPositionInLine + 1,
                EndColumn = charPositionInLine + 1,
                Message = msg,
                Severity = SeverityError
            });
        }

        public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            if (Errors.Count > 10)
                return;

            Errors.Add(new Marker()
            {
                StartLineNumber = line,
                EndLineNumber = line,
                StartColumn = charPositionInLine + 1,
                EndColumn = charPositionInLine + 1 + offendingSymbol.Text.Length,
                Message = msg,
                Severity = SeverityError
            });
        }

        public void Reset()
        {
            Errors.Clear();
        }
    }


    private BSPEntLexer _lexer = null!;
    private CommonTokenStream _tokenStream = null!;
    private BSPEntParser _parser = null!;
    private ErrorListener _errorListener = null!;

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        // Start up ANTLR parser
        _lexer = new BSPEntLexer(null);
        _tokenStream = new CommonTokenStream(_lexer);
        _parser = new BSPEntParser(_tokenStream);

        // Attach our custom error listener
        _errorListener = new ErrorListener();
        _lexer.RemoveErrorListeners();
        _parser.RemoveErrorListeners();
        _lexer.AddErrorListener(_errorListener);
        _parser.AddErrorListener(_errorListener);

        return Task.CompletedTask;
    }

    protected override Task OnParseAsync(string code, CancellationToken cancellationToken)
    {
        try
        {
            // Load up the new code
            var stream = new AntlrInputStream(code);
            _lexer.SetInputStream(stream);
            _tokenStream.SetTokenSource(_lexer);

            // Reset the parsing
            _lexer.Reset();
            _tokenStream.Reset();
            _parser.Reset();
            _errorListener.Reset();

            // Parse
            _parser.file();

            // Set the markers
            _markers = _errorListener.Errors;
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine($"Exception in BSPEntMonacoLanguageServer: {ex}");
        }

        return Task.CompletedTask;
    }

   
}
