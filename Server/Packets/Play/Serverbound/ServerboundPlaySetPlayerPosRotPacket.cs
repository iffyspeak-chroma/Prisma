using API.Core.Transform;
using API.Player.State;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlaySetPlayerPosRotPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        Location feet = new Location(packet.ReadDouble(), packet.ReadDouble(), packet.ReadDouble());
        Heading facing = new Heading(packet.ReadFloat(), packet.ReadFloat());
        PlayerPositionRotationFlags flags = (PlayerPositionRotationFlags) packet.ReadByte();
        
        // TODO: Probably do something with this packet
        return Task.CompletedTask;
    }
}