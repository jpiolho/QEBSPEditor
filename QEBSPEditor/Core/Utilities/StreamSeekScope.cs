namespace QEBSPEditor.Core.Utilities;

public class StreamSeekScope : IDisposable
{
    private long _originalOffset;
    private Stream _stream;

    public StreamSeekScope(Stream stream, long offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        _stream = stream;
        _originalOffset = stream.Position;
        stream.Seek(offset, origin);
    }

    public void Dispose()
    {
        _stream.Seek(_originalOffset, SeekOrigin.Begin);
    }
}
