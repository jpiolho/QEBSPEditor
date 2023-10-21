using QEBSPEditor.Core.Extensions;
using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

public class BSPFileGeneric : BSPFileBase, IBSPFile, IBSPFileEntities, IBSPSave
{
    public int Debug { get; set; }
    public BSPCapabilities Capabilities => BSPCapabilities.Entities | BSPCapabilities.Saveable;


    private byte[] _originalBytes;
    private string _entities = "";
    private ChunkHeader _entitiesHeader;
    private int _entitiesHeaderOffset;

    public string Entities { get => _entities; set => _entities = value; }
    public string VersionName => "Generic";

    public IBSPFile Load(Stream stream)
    {
        // First, try to find the entities chunk
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            _originalBytes = memoryStream.ToArray();
        }

        var array = _originalBytes;

        var idx = array.LocateFirst(Encoding.ASCII.GetBytes("\"worldspawn\""));

        if (idx == -1)
        {
            idx = array.LocateFirst(Encoding.ASCII.GetBytes("\"origin\""));

            if (idx == -1)
                throw new InvalidDataException("Could not find entities chunk");
        }

        // Find the beginning of the chunk
        while (idx-- > 0)
        {
            if (array[idx] == '{' && !char.IsWhiteSpace((char)array[idx - 1]) && array[idx - 1] != '}')
            {
                break;
            }
        }

        // Did we get a match?
        if (idx == -1)
            throw new InvalidDataException("Could not find entities chunk beginning");

        // Now find in the header, where the offset is
        using (var ms = new MemoryStream(_originalBytes))
        using (var reader = new BinaryReader(stream))
        {
            // Apply different offsets to try to match extra bytes if there's any
            for (var offset = 0; offset < 16; offset++)
            {
                var headerOffset = 4 + (2 * offset);
                stream.Seek(headerOffset, SeekOrigin.Begin);

                for (var i = 0; i < 50; i++)
                {
                    var header = ReadChunkHeader(reader);

                    // Found potential header?
                    if (header.Offset == idx)
                    {
                        var endChar = (char)_originalBytes[header.Offset + header.Size - 2];
                        if (endChar == '}' || char.IsWhiteSpace(endChar))
                        {
                            // Alright, this is probably it
                            stream.Seek(header.Offset, SeekOrigin.Begin);
                            _entities = Encoding.ASCII.GetString(reader.ReadBytes(header.Size));
                            _entitiesHeader = header;
                            _entitiesHeaderOffset = headerOffset;
                            return this;
                        }
                    }
                }
            }
        }

        throw new InvalidDataException("Could not find entities header");
    }

    public void Save(Stream stream)
    {
        using var writer = new BinaryWriter(stream);


        // First write all the original bytes
        writer.Write(_originalBytes);


        // Lets append our own entities at the end
        var offset = stream.Position;
        var entitiesAsBytes = Encoding.ASCII.GetBytes(_entities);
        writer.Write(entitiesAsBytes);
        writer.Write((char)0);

        // Then lets nullify the old entities chunk, to prevent it being found again
        stream.Seek(_entitiesHeader.Offset, SeekOrigin.Begin);
        for (var i = 0; i < _entitiesHeader.Size; i++)
            writer.Write((char)0);

        // Now lets change the header to our own
        stream.Seek(_entitiesHeaderOffset, SeekOrigin.Begin);
        writer.Write((int)offset);
        writer.Write(entitiesAsBytes.Length + 1);

        writer.Flush();

    }
}
