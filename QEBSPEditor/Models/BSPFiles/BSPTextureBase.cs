namespace QEBSPEditor.Models.BSPFiles;

public abstract class BSPTextureBase : IBSPTexture
{
    protected byte[] _data1 = Array.Empty<byte>();
    protected byte[] _data2 = Array.Empty<byte>();
    protected byte[] _data4 = Array.Empty<byte>();
    protected byte[] _data8 = Array.Empty<byte>();

    public string Name { get; set; } = "";
    public int Width { get; set; }
    public int Height { get; set; }
    public byte[] Data { get => _data1; set => _data1 = value; }
    public byte[] Data2 { get => _data2; set => _data2 = value; }
    public byte[] Data4 { get => _data4; set => _data4 = value; }
    public byte[] Data8 { get => _data4; set => _data8 = value; }

    public override int GetHashCode() => HashCode.Combine(Name, Width, Height, Data, Data2, Data4, Data8);
}
