namespace API.NBT;

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
}