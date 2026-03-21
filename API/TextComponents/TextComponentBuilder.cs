using System.Text.Json;
using System.Text.Json.Serialization;
using fNbt;

namespace API.TextComponents;

[JsonConverter(typeof(TextComponentConverter))]
public class TextComponentBuilder
{
    private readonly List<Dictionary<string, object>> _jsonComponents = new();
    private readonly List<NbtCompound> _nbtComponents = new();

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
        // For JSON Components
        var jComponent = new Dictionary<string, object>
        {
            { "text", text }
        };
        
        // For NBT Components
        NbtCompound nComponent = new NbtCompound()
        {
            new NbtString("text", text)
        };

        if (!string.IsNullOrEmpty(color))
        {
            jComponent["color"] = color;
            nComponent.Add(new NbtString("color", color));
        }

        if (bold.HasValue)
        {
            jComponent["bold"] = bold.Value;
            nComponent.Add(new NbtByte("bold", (byte) (bold.Value ? 1 : 0)));
        }

        if (italic.HasValue)
        {
            jComponent["italic"] = italic.Value;
            nComponent.Add(new NbtByte("italic", (byte) (italic.Value ? 1 : 0)));
        }

        if (underlined.HasValue)
        {
            jComponent["underlined"] = underlined.Value;
            nComponent.Add(new NbtByte("underlined", (byte) (underlined.Value ? 1 : 0)));
        }

        if (strikethrough.HasValue)
        {
            jComponent["strikethrough"] = strikethrough.Value;
            nComponent.Add(new NbtByte("strikethrough", (byte) (strikethrough.Value ? 1 : 0)));
        }
        
        if (clickEvent != null) jComponent["clickEvent"] = clickEvent.ToDictionary();
        if (hoverEvent != null) jComponent["hoverEvent"] = hoverEvent.ToDictionary();

        _jsonComponents.Add(jComponent);
        _nbtComponents.Add(nComponent);
        
        return this;
    }

    public TextComponentBuilder AddSpace()
    {
        _jsonComponents.Add(new Dictionary<string, object> { { "text", " " } });
        _nbtComponents.Add(new NbtCompound
        {
            new NbtString("text", " ")
        });
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
            { "extra", _jsonComponents }
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
    /// Builds an NbtFile Tag manually.
    /// </summary>
    public NbtFile ToNbtFile()
    {
        // Root Compound
        // |
        // |-> "extra" List
        // |    [] component compound
        // |    |
        // |    |-> "bold" byte
        // |    |
        // |    |-> "color" string
        // |    |
        // |    |-> "italic" byte
        // |    |
        // |    |-> "obfuscated" byte
        // |    |
        // |    |-> "strikethrough" byte
        // |    |
        // |    |-> "underlined" byte
        // |    |
        // |    |-> "text" string
        // |
        // |-> "text" String

        NbtFile file = new NbtFile();
        
        NbtString rootText = new NbtString("text", "");
        NbtList extraComponents = new NbtList("extra", NbtTagType.Compound);

        foreach (NbtCompound cmpnd in _nbtComponents)
        {
            extraComponents.Add(cmpnd);
        }
        
        file.RootTag.Add(extraComponents);
        file.RootTag.Add(rootText);
        
        return file;
    }

    /// <summary>
    /// Exposes raw components for converter use.
    /// </summary>
    public IReadOnlyList<Dictionary<string, object>> GetComponents() => _jsonComponents;
}
