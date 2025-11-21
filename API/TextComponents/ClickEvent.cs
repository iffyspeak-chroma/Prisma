namespace API.TextComponents;

public class ClickEvent
{
    public ClickEventAction Action { get; }
    public string Value { get; }

    public ClickEvent(ClickEventAction action, string value)
    {
        Action = action;
        Value = value;
    }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "action", Action.ToString().ToLower() },
            { "value", Value }
        };
    }
}