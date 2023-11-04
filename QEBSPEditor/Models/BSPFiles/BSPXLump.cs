namespace QEBSPEditor.Models.BSPFiles;

/// <summary>
/// Represents a single lump within BSPX data. A lump is a data block that represents specific types of content.
/// </summary>
public class BSPXLump
{
    /// <summary>
    /// Gets or sets the name of the lump.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the binary data of the lump.
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The offset in the file where this lump's data begins. This is internal and is used during the read/write process.
    /// </summary>
    internal int? EntryOffset { get; set; }
    /// <summary>
    /// The size of this lump's data in bytes. This is internal and is used during the read/write process.
    /// </summary>
    internal int? EntrySize { get; set; }

    /// <summary>
    /// Indicates whether the lump's has been loaded from a BSPX data chunk or created.
    /// </summary>
    public bool IsLoaded => EntryOffset != null && EntrySize != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="BSPXLump"/> class.
    /// </summary>
    public BSPXLump() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BSPXLump"/> class with the specified name and data.
    /// </summary>
    /// <param name="name">The name of the lump.</param>
    /// <param name="data">The binary data of the lump.</param>
    public BSPXLump(string name, byte[] data)
    {
        this.Name = name;
        this.Data = data;
    }
}
