namespace QEBSPEditor.Models.BSPFiles;

[Flags]
public enum BSPCapabilities
{
    None = 0,
    Saveable = 1 << 0,
    Entities = 1 << 1,
    Lighting = 1 << 2,
    Textures = 1 << 3,
    BSPX = 1 << 4
}
