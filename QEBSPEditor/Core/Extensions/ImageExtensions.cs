using QEBSPEditor.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QEBSPEditor.Core.Extensions;

public static class ImageExtensions
{
    public static byte[] ConvertToPalette(this Image<Rgb24> image, ColorPalette palette)
    {
        var data = new byte[image.Width * image.Height];
        image.ProcessPixelRows(accessor =>
        {
            int i = 0;
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);

                for (var x = 0; x < row.Length; x++, i++)
                {
                    ref var pixel = ref row[x];

                    var colorIdx = palette.FindColor(pixel.R, pixel.G, pixel.B);
                    if (colorIdx == -1)
                        throw new Exception("Failed to find matching color");

                    data[i] = (byte)colorIdx;
                }
            }
        });

        return data;
    }
}
