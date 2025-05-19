using System.Numerics;
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

    public static void WriteFixedSizeArray(this BinaryWriter writer, byte[] array, int length)
    {
        var len = Math.Min(length, array.Length);
        writer.Write(array[0..len]);

        // Pad the remaining with zeros
        for (var i = array.Length; i < len; i++)
            writer.Write((byte)0);
    }

    public static void Write(this BinaryWriter writer, Vector3 vector)
    {
        writer.Write(vector.X);
        writer.Write(vector.Y);
        writer.Write(vector.Z);
    }
}
