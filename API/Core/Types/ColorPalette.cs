namespace API.Core.Types;

public class ColorPalette
{
    public string Primary = "#ffffff";
    public string Secondary = "#000000";

    public ColorPalette(string primary, string secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }
}