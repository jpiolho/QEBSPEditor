using QEBSPEditor.Core.Extensions;
using QEBSPEditor.Core.Utilities;
using System.Text;

namespace QEBSPEditor.Models.WadFiles;

public class WADFile
{
    public class Entry
    {
        public required string Name { get; set; }
        public required int Width { get; set; }
        public required int Height { get; set; }
        public byte[] Data1 { get; set; } = Array.Empty<byte>();
        public byte[] Data2 { get; set; } = Array.Empty<byte>();
        public byte[] Data4 { get; set; } = Array.Empty<byte>();
        public byte[] Data8 { get; set; } = Array.Empty<byte>();
    }

    public List<Entry> Entries { get; set; } = new();



    public async Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.Default, true);


        writer.Write("WAD2".AsSpan());
        writer.Write((uint)Entries.Count);
        writer.Write((uint)writer.BaseStream.Position + sizeof(uint)); // We'll start writing the directory right after this

        for (var i = 0; i < Entries.Count; i++)
        {
            writer.Write((uint)0); // Offset
            writer.Write((uint)0); // Wad Size
            writer.Write((uint)0); // Size
            writer.Write((byte)0x44); // Type texture
            writer.Write((byte)0x00); // Compression
            writer.Write((ushort)0); // Skip bytes
            writer.WriteFixedSizeString(Entries[i].Name, 16);
        }

        for (var i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];

            var offset = writer.BaseStream.Position;

            writer.WriteFixedSizeString(entry.Name, 16); // Name
            writer.Write((uint)entry.Width);
            writer.Write((uint)entry.Height);

            var dataSize1 = entry.Width * entry.Height;
            var dataSize2 = (entry.Width / 2) * (entry.Height / 2);
            var dataSize4 = (entry.Width / 4) * (entry.Height / 4);
            var dataSize8 = (entry.Width / 8) * (entry.Height / 8);

            writer.Write((uint)40); // Offset to Data1
            writer.Write((uint)(40 + dataSize1)); // Offset to Data2
            writer.Write((uint)(40 + dataSize1 + dataSize2)); // Offset to Data4
            writer.Write((uint)(40 + dataSize1 + dataSize2 + dataSize4)); // Offset to Data8

            writer.WriteFixedSizeArray(entry.Data1, dataSize1);
            writer.WriteFixedSizeArray(entry.Data2, dataSize2);
            writer.WriteFixedSizeArray(entry.Data4, dataSize4);
            writer.WriteFixedSizeArray(entry.Data8, dataSize8);

            var size = writer.BaseStream.Position - offset;

            // Write in the directory
            using (new StreamSeekScope(writer.BaseStream, 12 + (32 * i)))
            {
                writer.Write((uint)offset);
                writer.Write((uint)size);
                writer.Write((uint)size);
            }
        }

        ms.Position = 0;
        await ms.CopyToAsync(stream, cancellationToken);
    }
}
