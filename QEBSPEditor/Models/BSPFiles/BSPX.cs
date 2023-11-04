using QEBSPEditor.Core.Extensions;
using QEBSPEditor.Core.Utilities;
using System.Text;

namespace QEBSPEditor.Models.BSPFiles;

/// <summary>
/// Represents BSPX data, which is a binary format for appending extra bits of information to a BSP file.
/// </summary>
public class BSPX
{
    /// <summary>
    /// Gets or sets the lumps contained within the BSPX.
    /// </summary>
    public List<BSPXLump> Lumps { get; set; } = new();

    /// <summary>
    /// Retrieves a lump by its name, if it exists.
    /// </summary>
    /// <param name="name">The name of the lump to retrieve.</param>
    /// <returns>The <see cref="BSPXLump"/> if found; otherwise, <c>null</c>.</returns>
    public BSPXLump? GetLump(string name)
    {
        return Lumps.SingleOrDefault(lump => lump.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if a lump with the specified name exists.
    /// </summary>
    /// <param name="name">The name of the lump to check for existence.</param>
    /// <returns><c>true</c> if the lump exists; otherwise, <c>false</c>.</returns>
    public bool HasLump(string name)
    {
        return GetLump(name) is not null;
    }

    /// <summary>
    /// Finds the index of the lump with the specified name.
    /// </summary>
    /// <param name="name">The name of the lump.</param>
    /// <returns>The index of the lump if found; otherwise, -1.</returns>
    public int FindIndex(string name)
    {
        return Lumps.FindIndex(lump => lump.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Removes the lump with the specified name.
    /// </summary>
    /// <param name="name">The name of the lump to remove.</param>
    /// <returns><c>true</c> if the lump was successfully removed; otherwise, <c>false</c>.</returns>
    public bool RemoveLump(string name)
    {
        var index = FindIndex(name);

        if (index == -1)
            return false;

        Lumps.RemoveAt(index);

        return true;
    }

    /// <summary>
    /// Adds a new lump or replaces an existing lump by name.
    /// </summary>
    /// <param name="lump">The lump to add or replace.</param>
    /// <returns>The lump that was added or replaced.</returns>
    public BSPXLump AddOrReplaceLump(BSPXLump lump)
    {
        var existingLump = GetLump(lump.Name);

        // Modify the data of the existing lump, if it exists
        if (existingLump != null)
        {
            existingLump.Data = lump.Data;
            return existingLump;
        }

        // Add new lump
        Lumps.Add(lump);
        return lump;
    }

    /// <summary>
    /// Loads BSPX data from the provided stream.
    /// </summary>
    /// <param name="stream">The stream from which to load the BSPX data.</param>
    /// <exception cref="InvalidDataException">Thrown if the stream does not contain a valid BSPX header.</exception>
    public void Load(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        // Read signature
        if (new string(reader.ReadChars(4)) != "BSPX")
            throw new InvalidDataException("Not a BSPX");

        // Read lump count
        var lumpCount = reader.ReadInt32();

        // Read the lump entries
        var lumps = new List<BSPXLump>(lumpCount);
        for (var i = 0; i < lumpCount; i++)
        {
            lumps.Add(new BSPXLump()
            {
                Name = reader.ReadFixedSizeString(24),
                EntryOffset = reader.ReadInt32(),
                EntrySize = reader.ReadInt32()
            });
        }

        // Read the lumps
        foreach (var lump in lumps)
        {
            stream.Seek(lump.EntryOffset!.Value, SeekOrigin.Begin);
            lump.Data = reader.ReadBytes(lump.EntrySize!.Value);
        }

        // Update variable
        this.Lumps = lumps;
    }

    /// <summary>
    /// Saves the BSPX data to the provided stream.
    /// </summary>
    /// <param name="stream">The stream to which the BSPX data will be saved.</param>
    public void Save(Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        // Write the BSPX header
        writer.Write("BSPX".AsSpan());
        writer.Write(Lumps.Count);

        // Write all the lump entries
        var listOffset = stream.Position;
        for (int i = 0; i < Lumps.Count; i++)
        {
            var lump = Lumps[i];

            writer.WriteFixedSizeString(lump.Name, 24);
            writer.Write((int)0);
            writer.Write(lump.Data.Length);
        }

        // Write the lumps
        for (int i = 0; i < Lumps.Count; i++)
        {
            var lump = Lumps[i];

            // Update the entry
            var offset = (int)stream.Position;
            using (var scope = new StreamSeekScope(stream, listOffset + (32 * i) + 24))
                writer.Write((int)offset);

            // Write the lump data
            writer.Write(lump.Data);
        }
    }

    /// <summary>
    /// Attempts to load BSPX data from the provided stream, and sets the out parameter to the loaded BSPX instance.
    /// </summary>
    /// <param name="stream">The stream from which to load the BSPX data.</param>
    /// <param name="bspx">The loaded BSPX instance if successful; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if loading is successful; otherwise, <c>false</c>.</returns>
    public static bool TryLoad(Stream stream, out BSPX bspx)
    {
        bspx = new BSPX();

        try
        {
            bspx.Load(stream);
            return true;
        }
        catch (InvalidDataException)
        {
        }

        return false;
    }
}
