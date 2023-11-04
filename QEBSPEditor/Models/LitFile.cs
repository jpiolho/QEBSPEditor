using System.Text;

namespace QEBSPEditor.Models;

public class LitFile
{
    public byte[] Data { get; set; } = Array.Empty<byte>();

    
    public void Load(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if(new string(reader.ReadChars(4)) != "QLIT")
            throw new InvalidDataException("Not a lit file");

        var version = reader.ReadInt32();
        if (version != 1)
            throw new InvalidDataException($"Unsupported lit version: {version}");

        Data = reader.ReadBytes((int)(stream.Length - stream.Position));
    }

    public void Save(Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write("QLIT".ToCharArray());
        writer.Write((int)1);
        writer.Write(Data);
    }

}
