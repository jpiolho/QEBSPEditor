namespace QEBSPEditor.Models;

public interface IBSPTexture
{
    string Name { get; set; } 
    int Width { get; set; }
    int Height { get; set; }
    byte[] Data { get; set; }
    byte[] Data2 { get; set; }
    byte[] Data4 { get; set; }
    byte[] Data8 { get; set; }
}
