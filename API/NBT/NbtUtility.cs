using fNbt;

namespace API.NBT;

public class NbtUtility
{
    public static byte[] ConvertToBytes(NbtCompound compound)
    {
        var file = new NbtFile(compound);
        using var stream = new MemoryStream();
        file.SaveToStream(stream, NbtCompression.None);

        return stream.ToArray();
    }
}