using API.Logging;
using API.Protocol.Mojang;
using fNbt;

namespace API.Core.Managers;

public class BlockManager
{
    public static int GetBlockId(Identifier id)
    {
        var blockRegistry = RegistryManager.GetRegistry(Identifier.Parse("minecraft:block"));

        if (blockRegistry == null)
            return DefaultToAir(id, "Block registry is null!");

        var block = RegistryManager.GetEntryFromRegistry(blockRegistry, id);

        if (block == null)
            return DefaultToAir(id, "Block is not in registry.");

        if (block.Data == null)
            return DefaultToAir(id, "Block is in registry but doesn't have any data assigned.");

        if (block.Data.TryGet<NbtInt>("protocol_id", out var nbtTag))
        {
            if (nbtTag == null)
                return DefaultToAir(id, "Block is in registry but does not have a protocol ID.");

            int bId = nbtTag.Value;
            return bId;
        }

        return DefaultToAir(id, "of an unknown reason.");
    }

    private static int DefaultToAir(Identifier id, string reason)
    {
        LogTool.Error($"Failed to find block with id {id} because:\n {reason}");
        LogTool.Error("Using air instead!");
        return 0;
    }
}