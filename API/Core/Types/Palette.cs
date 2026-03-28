namespace API.Core.Types;

public class Palette
{
    public string Primary = "#ffffff";
    public string Secondary = "#000000";

    public Palette(string primary, string secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }
}