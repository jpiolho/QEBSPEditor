using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

public class BSPFileBSP2 : BSPFileBase, IBSPFile, IBSPFileEntities, IBSPFileLighting, IBSPSave
{
    /* BSP2 support. 32bits instead of shorts for everything (bboxes use floats) */
    private const string VersionHeader = "BSP2";

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

    public string Entities { get; set; }
    public byte[] Planes { get; set; }
    public byte[] MipTex { get; set; }
    public byte[] Vertices { get; set; }
    public byte[] Visilist { get; set; }
    public byte[] Nodes { get; set; }
    public byte[] TexInfo { get; set; }
    public List<Face> Faces { get; set; }
    public byte[] Lightmaps { get; set; }
    public byte[] ClipNodes { get; set; }
    public byte[] Leaves { get; set; }
    public byte[] LFace { get; set; }
    public byte[] Edges { get; set; }
    public byte[] LEdges { get; set; }
    public byte[] Models { get; set; }

    public BSPCapabilities Capabilities => BSPCapabilities.Entities | BSPCapabilities.Lighting | BSPCapabilities.Saveable;
    public string VersionName => "BSP2";

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
    }

    public IBSPFile Load(Stream stream)
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

        ReadEntityChunk(headerEntities, this, reader);
        this.Planes = ReadGenericChunk(headerPlanes, reader);
        this.MipTex = ReadGenericChunk(headerMiptex, reader);
        this.Vertices = ReadGenericChunk(headerVertices, reader);
        this.Visilist = ReadGenericChunk(headerVisilist, reader);
        this.Nodes = ReadGenericChunk(headerNodes, reader);
        this.TexInfo = ReadGenericChunk(headerTexInfo, reader);
        ReadFaceChunk(headerFaces, this, reader);
        this.Lightmaps = ReadGenericChunk(headerLightmaps, reader);
        this.ClipNodes = ReadGenericChunk(headerClipnodes, reader);
        this.Leaves = ReadGenericChunk(headerLeaves, reader);
        this.LFace = ReadGenericChunk(headerLFace, reader);
        this.Edges = ReadGenericChunk(headerEdges, reader);
        this.LEdges = ReadGenericChunk(headerLEdges, reader);
        this.Models = ReadGenericChunk(headerModels, reader);

        return this;
    }




    private static void ReadEntityChunk(ChunkHeader header, BSPFileBSP2 file, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);

        file.Entities = Encoding.UTF8.GetString(reader.ReadBytes(header.Size - 1));
    }

    private static void ReadFaceChunk(ChunkHeader header, BSPFileBSP2 file, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);

        var total = header.Size / Face.SizeOf;

        var faces = new List<Face>(total);

        for (var i = 0; i < total; i++)
            faces.Add(Face.Read(reader));

        file.Faces = faces;
    }
}
