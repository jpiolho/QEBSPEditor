namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPFileLighting : IBSPFile
{
    byte[] Lightmaps { get; set; }
}
