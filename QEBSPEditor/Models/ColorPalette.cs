namespace QEBSPEditor.Models;

public class ColorPalette : ICloneable
{
    public string Name { get; set; } = "";
    public int[] Palette { get; set; } = Array.Empty<int>();
    public int FullBrightStart { get; set; } = 224;
    public int FullBrightEnd { get; set; } = 255;

    public int FindColor(byte r,byte g, byte b)
    {
        var num = (r << 16) | (g << 8) | b;
        return Array.IndexOf(Palette, num);
    }

    public Color GetColor(int index)
    {
        int color = Palette[index];
        return Color.FromRgb(
            (byte)(color >> 16 & 0xFF),
            (byte)(color >> 8 & 0xFF),
            (byte)(color & 0xFF)
        );
    }

    public Color[] GetAllColors()
    {
        var colors = new Color[Palette.Length];
        for(var i=0;i<Palette.Length; i++)
            colors[i] = GetColor(i);
        return colors;
    }

    public Color[] GetAllColorsWithoutFullbright()
    {
        return GetAllColors()[0..223];
    }

    public object Clone()
    {
        return new ColorPalette() { 
            Name = Name, 
            Palette = Palette.ToArray(),
            FullBrightStart = FullBrightStart,
            FullBrightEnd = FullBrightEnd,
        };
    }

    public override int GetHashCode() => HashCode.Combine(Name, Palette);
}
