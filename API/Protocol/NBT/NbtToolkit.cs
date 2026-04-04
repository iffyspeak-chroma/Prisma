using System.Text.Json;
using fNbt;

namespace API.Protocol.NBT;

public class NbtToolkit
{
    // Because of how modern minecraft works. You really don't need the short that tells minecraft how long the
    // root component name is. You'll never believe how many characters are in null.
    public static byte[] StripUnnecessary(byte[] raw)
    {
        using var input = new MemoryStream(raw);
        using var reader = new BinaryReader(input);

        using var output = new MemoryStream();
        using var writer = new BinaryWriter(output);

        byte tagType = reader.ReadByte();

        short nameLength = reader.ReadInt16();
        if (nameLength > 0)
        {
            reader.ReadBytes(nameLength);
        }

        writer.Write(tagType);

        writer.Write(reader.ReadBytes((int)(input.Length - input.Position)));

        return output.ToArray();
    }

    public static NbtCompound JsonToNbt(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = ParseElement(doc.RootElement) as NbtCompound;
        return root ?? new NbtCompound();
    }

    public static NbtTag ParseElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var compound = new NbtCompound();
                foreach (var prop in element.EnumerateObject())
                {
                    var child = ParseElement(prop.Value);
                    child.Name = prop.Name;
                    compound.Add(child);
                }
                return compound;

            case JsonValueKind.Array:
            {
                var items = element.EnumerateArray().Select(ParseElement).ToList();
                
                var firstType = items.FirstOrDefault()?.TagType ?? NbtTagType.String;
                bool mixed = items.Any(t => t.TagType != firstType);

                NbtList list = new NbtList();

                if (mixed)
                {
                    list.ListType = NbtTagType.String;
                    foreach (var item in items)
                    {
                        list.Add(new NbtString(item switch
                        {
                            NbtCompound c => c.ToString(),
                            NbtList l => l.ToString(),
                            NbtString s => s.Value,
                            NbtByte b => b.Value.ToString(),
                            NbtInt intItem => intItem.Value.ToString(),
                            NbtLong longItem => longItem.Value.ToString(),
                            NbtDouble doubleItem => doubleItem.Value.ToString(),
                            _ => item.ToString()
                        }));
                    }
                }
                else
                {
                    list.ListType = firstType;
                    foreach (var item in items)
                        list.Add(item);
                }

                return list;
            }

            case JsonValueKind.String:
                return new NbtString(element.GetString() ?? "");

            case JsonValueKind.Number:
            {
                if (element.TryGetInt32(out int intValue)) return new NbtInt(intValue);
                if (element.TryGetInt64(out long longValue)) return new NbtLong(longValue);
                if (element.TryGetDouble(out double doubleValue)) return new NbtDouble(doubleValue);
                return new NbtString(element.GetRawText());
            }

            case JsonValueKind.True:
            case JsonValueKind.False:
                return new NbtByte((byte)(element.GetBoolean() ? 1 : 0));

            default:
                throw new Exception($"Unsupported JSON value: {element.ValueKind}");
        }
    }
}