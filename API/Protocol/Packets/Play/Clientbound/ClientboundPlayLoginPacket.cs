using API.Core.Managers;
using API.Entities;
using API.Game;
using API.Player;
using API.Protocol.Mojang;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayLoginPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        
        packet = new Packet();

        // Create just some generic entity for the joining player.
        int entityId = EntityManager.RetrieveNextId();
        
        EntityManager.Instance.EntityList.Add(entityId, new GenericEntity(player.Uuid));
        player.AssociatedEntity = EntityManager.Instance.EntityList[entityId];
        
        packet.Write(entityId, false);
        
        // Whether we should consider the player in hardcore mode or not
        // TODO: Do a proper correct implementation
        packet.Write((entityId % 2 == 0));
        
        // Server's dimension names
        List<Identifier> dimensionList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:overworld"),
            Identifier.Parse("minecraft:the_nether"),
            Identifier.Parse("minecraft:the_end")
        };
        
        packet.Write(dimensionList.Count);
        foreach (Identifier dimension in dimensionList)
        {
            dimension.WriteToPacket(packet);
        }
        
        // Server's max player count
        // TODO: Correct implementation
        packet.Write(20);
        
        // Server's render distance
        // TODO: Correct implementation
        packet.Write(16);
        
        //and entity simulation distance
        packet.Write(16);
        
        // Server's debug status
        packet.Write(!Core.Server.Instance!.Configuration!.DebugMode);
        
        // Permit respawn screen
        packet.Write(true);
        
        // Use limited crafting (fucking boring)
        packet.Write(false);
        
        // Dimension Type
        packet.Write(0);
        
        // Dimension Name
        packet.Write("minecraft:overworld");
        
        // Hashed seed
        packet.Write(0L, asVarLong: false);
        
        // Game mode
        packet.Write(GameMode.Spectator);
        
        // Previous Game mode
        packet.Write(GameMode.Survival);
        
        // World debug mode
        packet.Write(false);
        
        // Flat world mode
        packet.Write(false);
        
        // Has death location
        packet.Write(false);
        
        // Portal tick cooldown
        packet.Write(5);
        
        // Sea level
        packet.Write(63);
        
        // Enforce Secure Chat (lmao fuck no)
        packet.Write(false);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:login"].Id);
        packet.WriteLength();

        await client.SendPacket(packet);
        
        await new ClientboundPlayDifficultyPacket().Call(context, null);
    }
}