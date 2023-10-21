namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPFile
{
    BSPCapabilities Capabilities { get; }
    IBSPFile Load(Stream stream);
    void Save(Stream stream);
}
