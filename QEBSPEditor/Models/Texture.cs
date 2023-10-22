namespace QEBSPEditor.Models;

public interface IBSPTexture
{
    string Name { get; set; } 
    int Width { get; set; }
    int Height { get; set; }
    byte[] Data { get; set; }
}
