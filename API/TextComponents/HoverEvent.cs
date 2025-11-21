namespace API.TextComponents;

public class HoverEvent
{
    public string Action { get; } = "show_text";
    public object Contents { get; }

    public HoverEvent(object contents)
    {
        Contents = contents;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "action", Action },
            { "contents", Contents }
        };
    }
}