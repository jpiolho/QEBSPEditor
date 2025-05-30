﻿using QEBSPEditor.Core.Extensions;
using QEBSPEditor.Core.Utilities;
using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

public class BSPFileBSP2 : BSPFileBase, IBSPFileEntities, IBSPFileLighting, IBSPSave, IBSPFileTextures, IBSPFileBSPX
{
    /* BSP2 support. 32bits instead of shorts for everything (bboxes use floats) */
    private const string VersionHeader = "BSP2";

    private static readonly Encoding EntityEncoding = Encoding.GetEncoding("Windows-1252");
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

    public class MipTexture : BSPTextureBase, IBSPWriteable
    {
        public static MipTexture Read(BinaryReader reader)
        {
            var offset = reader.BaseStream.Position;

            var tex = new MipTexture();
            tex.Name = reader.ReadFixedSizeString(16);
            tex.Width = reader.ReadInt32();
            tex.Height = reader.ReadInt32();

            Span<int> offsets = stackalloc int[4];
            for (var i = 0; i < offsets.Length; i++)
                offsets[i] = reader.ReadInt32();

            reader.BaseStream.Seek(offset + offsets[0], SeekOrigin.Begin);
            tex._data1 = reader.ReadBytes(tex.Width * tex.Height);

            reader.BaseStream.Seek(offset + offsets[1], SeekOrigin.Begin);
            tex._data2 = reader.ReadBytes((tex.Width / 2) * (tex.Height / 2));

            reader.BaseStream.Seek(offset + offsets[2], SeekOrigin.Begin);
            tex._data4 = reader.ReadBytes((tex.Width / 4) * (tex.Height / 4));

            reader.BaseStream.Seek(offset + offsets[3], SeekOrigin.Begin);
            tex._data8 = reader.ReadBytes((tex.Width / 8) * (tex.Height / 8));

            return tex;
        }

        public void Write(BinaryWriter writer)
        {
            var offsetBase = writer.BaseStream.Position;
            writer.WriteFixedSizeString(this.Name, 16);
            writer.Write((int)this.Width);
            writer.Write((int)this.Height);

            var offsets = writer.BaseStream.Position;
            writer.Write((int)0);
            writer.Write((int)0);
            writer.Write((int)0);
            writer.Write((int)0);

            var ofs1 = writer.BaseStream.Position - offsetBase;
            writer.Write(this._data1);
            var ofs2 = writer.BaseStream.Position - offsetBase;
            writer.Write(this._data2);
            var ofs4 = writer.BaseStream.Position - offsetBase;
            writer.Write(this._data4);
            var ofs8 = writer.BaseStream.Position - offsetBase;
            writer.Write(this._data8);


            // Write the offsets back
            using (new StreamSeekScope(writer.BaseStream, offsets, SeekOrigin.Begin))
            {
                writer.Write((int)ofs1);
                writer.Write((int)ofs2);
                writer.Write((int)ofs4);
                writer.Write((int)ofs8);
            }
        }

        public override object Clone()
        {
            return new MipTexture()
            {
                Name = this.Name,
                Data = (byte[])this.Data.Clone(),
                Data2 = (byte[])this.Data2.Clone(),
                Data4 = (byte[])this.Data4.Clone(),
                Data8 = (byte[])this.Data8.Clone(),
                Width = this.Width,
                Height = this.Height,
            };
        }
    }

    public class MipTextures : IBSPWriteable
    {
        public List<IBSPTexture?> Textures { get; set; } = new();

        public static MipTextures Read(BinaryReader reader)
        {
            var offset = reader.BaseStream.Position;

            var count = reader.ReadInt32();

            var offsets = new List<int>(count);
            for (var i = 0; i < count; i++)
                offsets.Add(reader.ReadInt32());

            var textures = new List<IBSPTexture?>(count);
            for (var i = 0; i < count; i++)
            {
                if (offsets[i] == -1)
                {
                    textures.Add(null);
                    continue;
                }

                reader.BaseStream.Seek(offset + offsets[i], SeekOrigin.Begin);

                textures.Add(MipTexture.Read(reader));
            }

            return new MipTextures()
            {
                Textures = textures
            };
        }

        public void Write(BinaryWriter writer)
        {
            var offsetBase = writer.BaseStream.Position;
            writer.Write((int)Textures.Count);

            var offsetListStart = writer.BaseStream.Position;
            for (var i = 0; i < Textures.Count; i++)
                writer.Write((int)0);

            List<long> offsets = new(Textures.Count);
            for (var i = 0; i < Textures.Count; i++)
            {
                var texture = Textures[i];
                if (texture is not null && texture is MipTexture mip)
                {
                    offsets.Add(writer.BaseStream.Position - offsetBase);
                    mip.Write(writer);
                }
                else
                {
                    offsets.Add(-1);
                }
            }

            // Write the offsets
            using (new StreamSeekScope(writer.BaseStream, offsetListStart))
            {
                for (var i = 0; i < offsets.Count; i++)
                    writer.Write((int)offsets[i]);
            }
        }
    }


    public string Entities { get; set; } = "";
    public byte[] Planes { get; set; } = Array.Empty<byte>();
    public MipTextures MipTex { get; set; } = new();
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

    public override BSPCapabilities Capabilities =>
        BSPCapabilities.Entities |
        BSPCapabilities.Lighting |
        BSPCapabilities.Saveable |
        BSPCapabilities.Textures |
        BSPCapabilities.BSPX;

    public override string VersionName => "BSP2";

    public List<IBSPTexture?> Textures { get => MipTex.Textures; set => throw new NotSupportedException(); }
    public BSPX? BSPXChunk { get; set; }

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
        this.MipTex = ReadMipTexChunk(headerMiptex, reader);
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

        // Lets try to read BSPX now
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

        if (BSPX.TryLoad(stream, out var bspx))
        {
            BSPXChunk = bspx;

            // Skip all the lumps
            if (bspx.Lumps.Count > 0)
                stream.Seek(bspx.Lumps.Max(l => l.EntryOffset!.Value + l.EntrySize!.Value), SeekOrigin.Begin);
        }

        // Read any extra bytes
        var extraBytesSize = (int)(stream.Length - stream.Position);
        if (extraBytesSize > 0)
            ExtraBytes = reader.ReadBytes(extraBytesSize);

        return this;
    }

    public void Save(Stream stream)
    {
        using var writer = new BinaryWriter(stream);

        writer.Write(VersionHeader.ToCharArray());
        for (var i = 0; i < ChunkHeader.SizeOf * 15; i++)
            writer.Write((byte)0);

        WriteChunkAndHeader(writer, 0, EntityEncoding.GetBytes(Entities), new byte[] { 0 });
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

        // Write BSPX
        if (BSPXChunk is not null)
        {
            stream.Seek(0, SeekOrigin.End);
            BSPXChunk.Save(stream);
        }

        // Write extra bytes
        stream.Seek(0, SeekOrigin.End);
        stream.Write(ExtraBytes);
    }

    private static MipTextures ReadMipTexChunk(ChunkHeader header, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);
        return MipTextures.Read(reader);
    }

    private static string ReadEntityChunk(ChunkHeader header, BinaryReader reader)
    {
        reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);

        return EntityEncoding.GetString(reader.ReadBytes(header.Size - 1));
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
}
