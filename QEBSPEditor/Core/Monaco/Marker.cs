namespace QEBSPEditor.Core.Monaco;

public class Marker
{
    public int StartLineNumber { get; set; }
    public int StartColumn { get; set; }
    public int EndLineNumber { get; set; }
    public int EndColumn { get; set; }
    public string? Message { get; set; }
    public int Severity { get; set; }
}