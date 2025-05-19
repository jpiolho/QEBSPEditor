using QEBSPEditor.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QEBSPEditor.Core.Extensions;

public static class IBSPTextureExtensions
{
    private static Rgb24[] ByteArrayToRgb24(byte[] array, ColorPalette palette)
    {
        return array
            .Select(index => palette.GetColor(index).ToPixel<Rgb24>())
            .ToArray();
    }

    public static Image<Rgb24> ToImage(this IBSPTexture texture, ColorPalette palette)
    {
        return Image.LoadPixelData<Rgb24>(ByteArrayToRgb24(texture.Data, palette), texture.Width, texture.Height);
    }

    public static Image<Rgb24> ToImageMipmap2(this IBSPTexture texture, ColorPalette palette)
    {
        return Image.LoadPixelData<Rgb24>(ByteArrayToRgb24(texture.Data2, palette), texture.Width / 2, texture.Height / 2);
    }

    public static Image<Rgb24> ToImageMipmap4(this IBSPTexture texture, ColorPalette palette)
    {
        return Image.LoadPixelData<Rgb24>(ByteArrayToRgb24(texture.Data4, palette), texture.Width / 4, texture.Height / 4);
    }

    public static Image<Rgb24> ToImageMipmap8(this IBSPTexture texture, ColorPalette palette)
    {
        return Image.LoadPixelData<Rgb24>(ByteArrayToRgb24(texture.Data8, palette), texture.Width / 8, texture.Height / 8);
    }

}
