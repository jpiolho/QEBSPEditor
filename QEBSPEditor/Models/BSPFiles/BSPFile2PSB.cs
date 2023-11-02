using QEBSPEditor.Core.Utilities;
using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

public class BSPFile2PSB : BSPFileBase, IBSPFileEntities, IBSPFileLighting, IBSPSave
{
    /* 
     * RMQ support (2PSB). 32bits instead of shorts for all but bbox sizes (which
     * still use shorts) 
    */
    private const string VersionHeader = "2PSB";

    public class Face : IBSPWriteable
    {
        public const int SizeOf = sizeof(uint) * 4 + sizeof(int) * 2 + sizeof(byte) * 4;

        public uint PlaneId { get; set; }
        public uint Side { get; set; }
        public int LEdgeId { get; set; }
        public uint LEdgeNum { get; set; }
        public uint TexInfoId { get; set; }
        public byte TypeLight { get; set; }
        public byte BaseLight { get; set; }
        public byte Light1 { get; set; }
        public byte Light2 { get; set; }
        public int LightmapId { get; set; }

        public static Face Read(BinaryReader reader)
        {
            return new Face()
            {
                PlaneId = reader.ReadUInt32(),
                Side = reader.ReadUInt32(),
                LEdgeId = reader.ReadInt32(),
                LEdgeNum = reader.ReadUInt32(),
                TexInfoId = reader.ReadUInt32(),
                TypeLight = reader.ReadByte(),
                BaseLight = reader.ReadByte(),
                Light1 = reader.ReadByte(),
                Light2 = reader.ReadByte(),
                LightmapId = reader.ReadInt32()
            };
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(PlaneId);
            writer.Write(Side);
            writer.Write(LEdgeId);
            writer.Write(LEdgeNum);
            writer.Write(TexInfoId);
            writer.Write(TypeLight);
            writer.Write(BaseLight);
            writer.Write(Light1);
            writer.Write(Light2);
            writer.Write(LightmapId);
        }
    }

    public string Entities { get; set; } = "";
    public byte[] Planes { get; set; } = Array.Empty<byte>();
    public byte[] MipTex { get; set; } = Array.Empty<byte>();
    public byte[] Vertices { get; set; } = Array.Empty<byte>();
    public byte[] Visilist { get; set; } = Array.Empty<byte>();
    public byte[] Nodes { get; set; } = Array.Empty<byte>();
    public byte[] TexInfo { get; set; } = Array.Empty<byte>();
    public List<Face> Faces { get; set; } = new();
    public byte[] Lightmaps { get; set; } = Array.Empty<byte>();
    public byte[] ClipNodes { get; set; } = Array.Empty<byte>();
    public byte[] Leaves { get; set; } = Array.Empty<byte>();
    public byte[] LFace { get; set; } = Array.Empty<byte>();
    public byte[] Edges { get; set; } = Array.Empty<byte>();
    public byte[] LEdges { get; set; } = Array.Empty<byte>();
    public byte[] Models { get; set; } = Array.Empty<byte>();

    public override BSPCapabilities Capabilities => BSPCapabilities.Entities | BSPCapabilities.Lighting | BSPCapabilities.Saveable;
    public override string VersionName => "2PSB";

    public void Save(Stream stream)
    {
        using var writer = new BinaryWriter(stream);

        writer.Write(VersionHeader.ToCharArray());
        for (var i = 0; i < ChunkHeader.SizeOf * 15; i++)
            writer.Write((byte)0);

        WriteChunkAndHeader(writer, 0, Encoding.UTF8.GetBytes(Entities), new byte[] { 0 });
        WriteChunkAndHeader(writer, 1, Planes);
        WriteChunkAndHeader(writer, 2, MipTex);
        WriteChunkAndHeader(writer, 3, Vertices);
        WriteChunkAndHeader(writer, 4, Visilist);
        WriteChunkAndHeader(writer, 5, Nodes);
        WriteChunkAndHeader(writer, 6, TexInfo);
        WriteChunkAndHeader(writer, 7, Faces);
        WriteChunkAndHeader(writer, 8, Lightmaps);
        WriteChunkAndHeader(writer, 9, ClipNodes);
        WriteChunkAndHeader(writer, 10, Leaves);
        WriteChunkAndHeader(writer, 11, LFace);
        WriteChunkAndHeader(writer, 12, Edges);
        WriteChunkAndHeader(writer, 13, LEdges);
        WriteChunkAndHeader(writer, 14, Models);

        // Write extra bytes
        stream.Seek(0, SeekOrigin.End);
        stream.Write(ExtraBytes);
    }

    public override IBSPFile Load(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        var version = new string(reader.ReadChars(4));
        if (version != VersionHeader)
            throw new InvalidDataException($"Unsupported BSP version: {version}");

        var headerEntities = ReadChunkHeader(reader);
        var headerPlanes = ReadChunkHeader(reader);
        var headerMiptex = ReadChunkHeader(reader);
        var headerVertices = ReadChunkHeader(reader);
        var headerVisilist = ReadChunkHeader(reader);
        var headerNodes = ReadChunkHeader(reader);
        var headerTexInfo = ReadChunkHeader(reader);
        var headerFaces = ReadChunkHeader(reader);
        var headerLightmaps = ReadChunkHeader(reader);
        var headerClipnodes = ReadChunkHeader(reader);
        var headerLeaves = ReadChunkHeader(reader);
        var headerLFace = ReadChunkHeader(reader);
        var headerEdges = ReadChunkHeader(reader);
        var headerLEdges = ReadChunkHeader(reader);
        var headerModels = ReadChunkHeader(reader);

        this.Entities = ReadEntityChunk(headerEntities, reader);
        this.Planes = ReadGenericChunk(headerPlanes, reader);
        this.MipTex = ReadGenericChunk(headerMiptex, reader);
        this.Vertices = ReadGenericChunk(headerVertices, reader);
        this.Visilist = ReadGenericChunk(headerVisilist, reader);
        this.Nodes = ReadGenericChunk(headerNodes, reader);
        this.TexInfo = ReadGenericChunk(headerTexInfo, reader);
        this.Faces = ReadFaceChunk(headerFaces, reader);
        this.Lightmaps = ReadGenericChunk(headerLightmaps, reader);
        this.ClipNodes = ReadGenericChunk(headerClipnodes, reader);
        this.Leaves = ReadGenericChunk(headerLeaves, reader);
        this.LFace = ReadGenericChunk(headerLFace, reader);
        this.Edges = ReadGenericChunk(headerEdges, reader);
        this.LEdges = ReadGenericChunk(headerLEdges, reader);
        this.Models = ReadGenericChunk(headerModels, reader);

        // Read any extra bytes, as it can contain BSPX or other data
        stream.Seek(MathUtils.Max(
            headerEntities.EndOffset,
            headerPlanes.EndOffset,
            headerMiptex.EndOffset,
            headerVertices.EndOffset,
            headerVisilist.EndOffset,
            headerNodes.EndOffset,
            headerTexInfo.EndOffset,
            headerFaces.EndOffset,
            headerLightmaps.EndOffset,
            headerClipnodes.EndOffset,
            headerLeaves.EndOffset,
            headerLFace.EndOffset,
            headerEdges.EndOffset,
            headerLEdges.EndOffset,
            headerModels.EndOffset
        ), SeekOrigin.Begin);
        var extraBytesSize = (int)(stream.Length - stream.Position);
        if (extraBytesSize > 0)
            ExtraBytes = reader.ReadBytes(extraBytesSize);

        return this;
    }



    #region Load Methods

    private static string ReadEntityChunk(ChunkHeader header, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);

        return Encoding.UTF8.GetString(reader.ReadBytes(header.Size - 1));
    }

    private static List<Face> ReadFaceChunk(ChunkHeader header, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);

        var total = header.Size / Face.SizeOf;

        var faces = new List<Face>(total);

        for (var i = 0; i < total; i++)
            faces.Add(Face.Read(reader));

        return faces;
    }

    #endregion
}
