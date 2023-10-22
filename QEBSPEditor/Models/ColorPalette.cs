namespace QEBSPEditor.Models;

public class ColorPalette : ICloneable
{
    public string Name { get; set; } = "";
    public int[] Palette { get; set; } = Array.Empty<int>();


    public Color GetColor(int index)
    {
        int color = Palette[index];
        return Color.FromRgb(
            (byte)(color >> 16 & 0xFF),
            (byte)(color >> 8 & 0xFF),
            (byte)(color & 0xFF)
        );
    }

    public object Clone()
    {
        return new ColorPalette() { 
            Name = Name, 
            Palette = Palette.ToArray()
        };
    }
}
