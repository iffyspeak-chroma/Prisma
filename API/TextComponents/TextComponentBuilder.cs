using System.Text.Json;
using System.Text.Json.Serialization;
using fNbt;

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

    private NbtCompound ConvertDictionary(string name, Dictionary<string, object> dict)
    {
        var compound = new NbtCompound(name);

        foreach (var kv in dict)
        {
            switch (kv.Value)
            {
                case string s:
                    compound.Add(new NbtString(kv.Key, s));
                    break;

                case bool b:
                    compound.Add(new NbtByte(kv.Key, (byte)(b ? 1 : 0)));
                    break;

                case Dictionary<string, object> inner:
                    compound.Add(ConvertDictionary(kv.Key, inner));
                    break;
            }
        }

        return compound;
    }

    /// <summary>
    /// Builds the NbtCompound manually.
    /// </summary>
    public NbtCompound ToNbt()
    {
        var root = new NbtCompound("");
        
        root.Add(new NbtString("text", ""));
        
        var extraList = new NbtList("extra", NbtTagType.Compound);

        foreach (var comp in _components)
        {
            var compound = new NbtCompound();

            foreach (var kv in comp)
            {
                switch (kv.Value)
                {
                    case string s:
                    {
                        compound.Add(new NbtString(kv.Key, s));
                        break;
                    }

                    case bool b:
                    {
                        compound.Add(new NbtByte(kv.Key, (byte)(b ? 1 : 0)));
                        break;
                    }

                    case Dictionary<string, object> dict:
                    {
                        compound.Add(ConvertDictionary(kv.Key, dict));
                        break;
                    }
                }
            }

            extraList.Add(compound);
        }
        
        root.Add(extraList);

        return root;
    }

    /// <summary>
    /// Exposes raw components for converter use.
    /// </summary>
    public IReadOnlyList<Dictionary<string, object>> GetComponents() => _components;
}
