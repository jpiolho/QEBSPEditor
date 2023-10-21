namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPFile
{
    string Name { get; set; }
    string VersionName { get; }
    BSPCapabilities Capabilities { get; }
    IBSPFile Load(Stream stream);
}
