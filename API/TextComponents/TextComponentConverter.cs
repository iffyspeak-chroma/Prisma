using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.TextComponents;

public class TextComponentConverter : JsonConverter<TextComponentBuilder>
{
    public override TextComponentBuilder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
    
    public override void Write(Utf8JsonWriter writer, TextComponentBuilder value, JsonSerializerOptions options)
    {
        var obj = value.ToObject();
        JsonSerializer.Serialize(writer, obj, options);
    }
}