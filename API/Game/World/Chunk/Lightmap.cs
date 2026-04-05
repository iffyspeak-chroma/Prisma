using API.Core.Types;
using API.Protocol.Packets;

namespace API.Game.World.Chunk;

public class Lightmap : ISerializable
{
    public BitSet SkyLightMask { get; set; } = new();
    public BitSet BlockLightMask { get; set; } = new();
    public BitSet EmptySkyLightMask { get; set; } = new();
    public BitSet EmptyBlockLightMask { get; set; } = new();

    public List<byte[]> SkyLightArrays { get; set; } = new();
    public List<byte[]> BlockLightArrays { get; set; } = new();
    
    public void Serialize(Packet packet)
    {
        SkyLightMask.Serialize(packet);
        BlockLightMask.Serialize(packet);
        EmptySkyLightMask.Serialize(packet);
        EmptyBlockLightMask.Serialize(packet);
        
        WriteLightArray(packet, SkyLightArrays);
        WriteLightArray(packet, BlockLightArrays);
    }

    private void WriteLightArray(Packet p, List<byte[]> arrays)
    {
        p.Write(arrays.Count);
        foreach (var arr in arrays)
        {
            p.Write(arr.Length);
            p.Write(arr);
        }
    }
}