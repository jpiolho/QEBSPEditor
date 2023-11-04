using QEBSPEditor.Core.Extensions;
using QEBSPEditor.Core.Utilities;
using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

public class BSPXLump
{
    public string Name { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();

    internal int? Offset { get; set; }
    internal int? Size { get; set; }

    public void Load(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        this.Name = reader.ReadFixedSizeString(24);

        Offset = reader.ReadInt32();
        Size = reader.ReadInt32();

        using (var scope = new StreamSeekScope(stream, Offset.Value))
        {
            this.Data = reader.ReadBytes(Size.Value);
        }
    }
}
