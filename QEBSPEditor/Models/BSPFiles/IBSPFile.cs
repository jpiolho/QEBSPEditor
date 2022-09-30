namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPFile
{
    string Entities { get; set; }
    byte[] Lightmaps { get; set; }

    IBSPFile Load(Stream stream);
    void Save(Stream stream);
}
