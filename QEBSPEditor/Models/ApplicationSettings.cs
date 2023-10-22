namespace QEBSPEditor.Models;

public class ApplicationSettings
{
    public int Version { get; set; } = 1;
    public bool ExperimentalMode { get; set; } = false;
    public List<ColorPalette> ColorPalettes { get; set; } = new();
    public bool EntitiesPreferSource { get; set; } = false;
}
