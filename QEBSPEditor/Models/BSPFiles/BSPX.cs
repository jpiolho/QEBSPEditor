using QEBSPEditor.Core.Extensions;
using QEBSPEditor.Core.Utilities;
using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

public class BSPX
{
    public List<BSPXLump> Lumps { get; set; } = new();


    public BSPXLump? GetLump(string name)
    {
        return Lumps.SingleOrDefault(lump => lump.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasLump(string name)
    {
        return GetLump(name) is not null;
    }

    public BSPXLump AddOrReplaceLump(BSPXLump lump)
    {
        var existingLump = GetLump(lump.Name);

        // Modify the data of the existing lump, if it exists
        if(existingLump != null)
        {
            existingLump.Data = lump.Data;
            return existingLump;
        }

        // Add new lump
        Lumps.Add(lump);
        return lump;
    }

    public void Load(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (new string(reader.ReadChars(4)) != "BSPX")
            throw new InvalidDataException("Not a BSPX");

        var lumpCount = reader.ReadInt32();
        Lumps = new List<BSPXLump>(lumpCount);
        
        for(var i=0;i< lumpCount;i++)
        {
            var lump = new BSPXLump();
            lump.Load(stream);

            Lumps.Add(lump);
        }
    }

    public void Save(Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write("BSPX".AsSpan());
        writer.Write(Lumps.Count);

        // Write all the lump headers
        var listOffset = stream.Position;
        for (int i = 0; i < Lumps.Count; i++)
        {
            BSPXLump lump = Lumps[i];

            writer.WriteFixedSizeString(lump.Name, 24);
            writer.Write((int)0);
            writer.Write(lump.Data.Length);
        }

        // Write the lumps
        for (int i = 0; i < Lumps.Count; i++)
        {
            BSPXLump lump = Lumps[i];

            var lumpOffset = stream.Position;
            using (var scope = new StreamSeekScope(stream, listOffset + (32 * i) + 24))
                writer.Write((int)lumpOffset);
            writer.Write(lump.Data);
        }
    }


    public static bool TryLoadBSPXLump(Stream stream, out BSPX bspx)
    {
        bspx = new BSPX();

        try
        {
            bspx.Load(stream);
            return true;
        }
        catch(InvalidDataException)
        {
        }

        return false;
    }
}
