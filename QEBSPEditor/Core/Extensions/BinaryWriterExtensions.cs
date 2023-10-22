using System.Text;

namespace QEBSPEditor.Core.Extensions;

public static class BinaryWriterExtensions
{
    public static void WriteFixedSizeString(this BinaryWriter writer, string text, int length)
    {
        Span<byte> bytes = stackalloc byte[length];
        Encoding.ASCII.GetBytes(text).CopyTo(bytes);
        writer.Write(bytes);
    }
}
