namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPSave : IBSPFile
{
    void Save(Stream stream);
}
