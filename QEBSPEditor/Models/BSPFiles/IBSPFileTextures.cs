namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPFileTextures : IBSPFile
{
    List<IBSPTexture?> Textures { get; set; }
}
