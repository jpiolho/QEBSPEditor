namespace QEBSPEditor.Models.BSPFiles;

public interface IBSPFileBSPX : IBSPFile
{
    BSPX? BSPXChunk { get; set; }
}
