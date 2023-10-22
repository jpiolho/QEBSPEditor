using System.Text;

namespace QEBSPEditor.Core.Extensions;

public static class BinaryReaderExtensions
{
    public static string ReadFixedSizeString(this BinaryReader reader, int length)
    {
        var bytes = reader.ReadBytes(length);

        int size = 0;
        for(size=0;size<length;size++)
        {
            if (bytes[size] == 0)
                break;
        }

        return Encoding.ASCII.GetString(bytes, 0, size-1);
    }
}
