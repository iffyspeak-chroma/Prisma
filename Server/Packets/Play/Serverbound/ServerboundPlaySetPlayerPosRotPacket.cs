using API.DataTypes;
using API.DataTypes.Entities;
using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlaySetPlayerPosRotPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        Location feet = new Location(packet.ReadDouble(), packet.ReadDouble(), packet.ReadDouble());
        Heading facing = new Heading(packet.ReadFloat(), packet.ReadFloat());
        PlayerPositionRotationFlags flags = (PlayerPositionRotationFlags) packet.ReadByte();
        
        // TODO: Probably do something with this packet
    }
}