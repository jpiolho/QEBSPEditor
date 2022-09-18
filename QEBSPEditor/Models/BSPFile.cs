using System.Text;

namespace QEBSPEditor.Models
{
    public class BSPFile
    {
        private struct ChunkHeader
        {
            public const int SizeOf = sizeof(int) * 2;

            public int Offset { get; set; }
            public int Size { get; set; }
        }

        public string Entities { get; set; }
        public byte[] Planes { get; set; }
        public byte[] MipTex { get; set; }
        public byte[] Vertices { get; set; }
        public byte[] Visilist { get; set; }
        public byte[] Nodes { get; set; }
        public byte[] TexInfo { get; set; }
        public byte[] Faces { get; set; }
        public byte[] Lightmaps { get; set; }
        public byte[] ClipNodes { get; set; }
        public byte[] Leaves { get; set; }
        public byte[] LFace { get; set; }
        public byte[] Edges { get; set; }
        public byte[] LEdges { get; set; }
        public byte[] Models { get; set; }


        public void SaveToStream(Stream stream)
        {
            using var writer = new BinaryWriter(stream);

            writer.Write((int)29);
            for(var i=0;i< ChunkHeader.SizeOf * 15;i++)
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

        public static BSPFile LoadFromStream(Stream stream)
        {
            using var reader = new BinaryReader(stream);

            var version = reader.ReadInt32();
            if (version != 29)
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

            BSPFile file = new BSPFile();

            ReadEntityChunk(headerEntities, file, reader);
            file.Planes = ReadGenericChunk(headerPlanes, file, reader);
            file.MipTex = ReadGenericChunk(headerMiptex, file, reader);
            file.Vertices = ReadGenericChunk(headerVertices, file, reader);
            file.Visilist = ReadGenericChunk(headerVisilist, file, reader);
            file.Nodes = ReadGenericChunk(headerNodes, file, reader);
            file.TexInfo = ReadGenericChunk(headerTexInfo, file, reader);
            file.Faces = ReadGenericChunk(headerFaces, file, reader);
            file.Lightmaps = ReadGenericChunk(headerLightmaps, file, reader);
            file.ClipNodes = ReadGenericChunk(headerClipnodes, file, reader);
            file.Leaves = ReadGenericChunk(headerLeaves, file, reader);
            file.LFace = ReadGenericChunk(headerLFace, file, reader);
            file.Edges = ReadGenericChunk(headerEdges, file, reader);
            file.LEdges = ReadGenericChunk(headerLEdges, file, reader);
            file.Models = ReadGenericChunk(headerModels, file, reader);

            return file;
        }


        private static byte[] ReadGenericChunk(ChunkHeader header, BSPFile file,BinaryReader reader)
        {
            reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);
            return reader.ReadBytes(header.Size);
        }

        private static void ReadEntityChunk(ChunkHeader header, BSPFile file, BinaryReader reader)
        {
            reader.BaseStream.Seek(header.Offset, SeekOrigin.Begin);

            file.Entities = Encoding.UTF8.GetString(reader.ReadBytes(header.Size-1));
        }

        private static ChunkHeader ReadChunkHeader(BinaryReader reader)
        {
            return new ChunkHeader() { Offset = reader.ReadInt32(), Size = reader.ReadInt32() };
        }

        private static void WriteChunkAndHeader(BinaryWriter writer, int headerNum,byte[] content, byte[]? append = null)
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
            writer.Seek(sizeof(int) + (headerNum * ChunkHeader.SizeOf), SeekOrigin.Begin);
            writer.Write((int)pos);
            writer.Write((int)length);
        }
    }
}
