namespace QEBSPEditor.Models.BSPFiles;

public abstract class BSPFileBase : IBSPFile
{
    public string Name { get; set; } = "";

    public abstract string VersionName { get; }
    public abstract BSPCapabilities Capabilities { get; }
    public abstract IBSPFile Load(Stream stream);

    public byte[] ExtraBytes { get; set; } = Array.Empty<byte>();

    protected interface IBSPWriteable
    {
        void Write(BinaryWriter writer);
    }

    protected struct ChunkHeader
    {
        public const int SizeOf = sizeof(int) * 2;

        public int Offset { get; set; }
        public int Size { get; set; }


        public int EndOffset => Offset + Size;
    }

    protected static ChunkHeader ReadChunkHeader(BinaryReader reader)
    {
        return new ChunkHeader() { Offset = reader.ReadInt32(), Size = reader.ReadInt32() };
    }

    protected static byte[] ReadGenericChunk(ChunkHeader header, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);
        return reader.ReadBytes(header.Size);
    }

    protected static void WriteChunkAndHeader<TItem>(BinaryWriter writer, int headerNum, TItem content) where TItem : IBSPWriteable
    {
        // Write content
        writer.Seek(0, SeekOrigin.End);
        var pos = writer.BaseStream.Position;

        content.Write(writer);

        var length = writer.BaseStream.Position - pos;

        // Write header
        writer.Seek(sizeof(int) + headerNum * ChunkHeader.SizeOf, SeekOrigin.Begin);
        writer.Write((int)pos);
        writer.Write((int)length);
    }
    protected static void WriteChunkAndHeader<TItem>(BinaryWriter writer, int headerNum, List<TItem> content) where TItem : IBSPWriteable
    {
        // Write content
        writer.Seek(0, SeekOrigin.End);
        var pos = writer.BaseStream.Position;

        foreach (var item in content)
            item.Write(writer);

        var length = writer.BaseStream.Position - pos;

        // Write header
        writer.Seek(sizeof(int) + headerNum * ChunkHeader.SizeOf, SeekOrigin.Begin);
        writer.Write((int)pos);
        writer.Write((int)length);
    }
    protected static void WriteChunkAndHeader(BinaryWriter writer, int headerNum, byte[] content, byte[]? append = null)
    {
        int length = content.Length;

        // Write content
        writer.Seek(0, SeekOrigin.End);
        var pos = writer.BaseStream.Position;
        writer.Write(content);

        if (append != null)
        {
            writer.Write(append);
            length += append.Length;
        }

        // Write header
        writer.Seek(sizeof(int) + headerNum * ChunkHeader.SizeOf, SeekOrigin.Begin);
        writer.Write((int)pos);
        writer.Write(length);
    }
}
