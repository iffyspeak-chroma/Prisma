using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlayLoginPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        packet = new Packet();
        
        // Create just some generic entity for the joining player.
        int entityId = EntityManager.RetrieveNextId();
        EntityManager.Instance.EntityList.Add(entityId,new GenericEntity());
        player.AssociatedEntity = EntityManager.Instance.EntityList[entityId];
        
        packet.Write(entityId, false);
        
    }
}