using System.Text.Json;
using System.Text.Json.Serialization;
using API.NBT;

namespace API.TextComponents;

[JsonConverter(typeof(TextComponentConverter))]
public class TextComponentBuilder
{
    private readonly List<Dictionary<string, object>> _components = new();

    public TextComponentBuilder AddText(
        string text,
        string color = null,
        bool? bold = null,
        bool? italic = null,
        bool? underlined = null,
        bool? strikethrough = null,
        ClickEvent clickEvent = null,
        HoverEvent hoverEvent = null)
    {
        var component = new Dictionary<string, object>
        {
            { "text", text }
        };

        if (!string.IsNullOrEmpty(color)) component["color"] = color;
        if (bold.HasValue) component["bold"] = bold.Value;
        if (italic.HasValue) component["italic"] = italic.Value;
        if (underlined.HasValue) component["underlined"] = underlined.Value;
        if (strikethrough.HasValue) component["strikethrough"] = strikethrough.Value;
        if (clickEvent != null) component["clickEvent"] = clickEvent.ToDictionary();
        if (hoverEvent != null) component["hoverEvent"] = hoverEvent.ToDictionary();

        _components.Add(component);
        return this;
    }

    public TextComponentBuilder AddSpace()
    {
        _components.Add(new Dictionary<string, object> { { "text", " " } });
        return this;
    }

    /// <summary>
    /// Returns the raw object structure used for serialization.
    /// </summary>
    public object ToObject()
    {
        return ToDictionary();
    }

    /// <summary>
    /// Returns the dictionary structure used for serialization.
    /// </summary>
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "text", "" },
            { "extra", _components }
        };
    }

    /// <summary>
    /// Builds the JSON string manually.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(ToObject(), new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Builds an NbtCompound Tag manually.
    /// </summary>
    public CompoundTag ToNbtCompound()
    {
        CompoundTag root = new CompoundTag(null, true);
        
        root.Children.Add(new StringTag("text", ""));

        CompoundTag extras = new CompoundTag("extra", false);

        foreach (KeyValuePair<string, object> kvp in ToDictionary())
        {
            if (kvp.Value != null)
            {
                extras.Children.Add(new StringTag(kvp.Key.ToString().ToLower(), kvp.Value.ToString().ToLower()));
            }
        }
        
        root.Children.Add(extras);
        root.AssemblePayload();

        return root;
    }

    /// <summary>
    /// Exposes raw components for converter use.
    /// </summary>
    public IReadOnlyList<Dictionary<string, object>> GetComponents() => _components;
}
