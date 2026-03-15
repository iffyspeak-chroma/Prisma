using fNbt;

namespace API.NBT;

public class NbtUtility
{
    public static byte[] ConvertToBytes(NbtCompound compound)
    {
        var file = new NbtFile(compound);

        using var stream = new MemoryStream();
        file.SaveToStream(stream, NbtCompression.None);

        var data = stream.ToArray();

        // Remove the extra TAG_End written by NbtFile
        if (data.Length > 0 && data[^1] == 0x00)
        {
            Array.Resize(ref data, data.Length - 1);
        }

        return data;
    }
}