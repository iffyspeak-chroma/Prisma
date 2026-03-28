using API.Player;
using API.Player.State;
using API.Protocol.Packets;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace API.Protocol.Networking;

public class NetworkedClient
{
    public bool IsConnected { get; set; }
    public bool IsCompressing { get; set; }
    public bool IsEncrypting { get; set; }
    public IChannel Channel { get; set; }
    public PlayerGamestate Gamestate { get; set; }
    public PlayerConnectionInfo PlayerConnectionInfo { get; set; }
    public ServerPlayer Player { get; set; }
    public DisconnectReason DisconnectReason { get; set; } = Networking.DisconnectReason.Generic;

    #region Heartbeat

        public long LastHeartbeat { get; set; }

        private int timeoutStart = 15;
        private int timeoutRemaining;
        private object timeoutLock = new();

        private bool heartbeatActive;
        private bool timeoutActive;

        public event Action<NetworkedClient> TransmitHeartbeat;
        public event Action<NetworkedClient> TimeoutReached;
        
        public void StartHeartbeat()
        {
            heartbeatActive = true;
            _ = AsyncHeartbeatLoop();
        }

        private async Task AsyncHeartbeatLoop()
        {
            while (heartbeatActive)
            {
                await Task.Delay(1000);
                TransmitHeartbeat?.Invoke(this);
            }
        }

        public void StartTimeoutCountdown()
        {
            lock (timeoutLock)
            {
                timeoutRemaining = timeoutStart;
            }

            timeoutActive = true;
        }

        private async Task AsyncTimeoutLoop()
        {
            while (timeoutActive)
            {
                await Task.Delay(1000);

                bool timedOut = false;
                lock (timeoutLock)
                {
                    timeoutRemaining--;
                    if (timeoutRemaining <= 0)
                    {
                        timedOut = true;
                    }

                    if (timedOut)
                    {
                        TimeoutReached?.Invoke(this);
                        StopTimers();
                    }
                }
            }
        }

        public void ResetTimeout()
        {
            lock (timeoutLock)
            {
                timeoutRemaining = timeoutStart;
            }
        }

        public void StopTimers()
        {
            heartbeatActive = false;
            timeoutActive = false;
        }

    #endregion
    
    public NetworkedClient(IChannel channel, PlayerConnectionInfo pci)
    {
        Channel = channel;
        PlayerConnectionInfo = pci;
        IsConnected = false;
        IsCompressing = false;
        IsEncrypting = false;
    }

    public NetworkedClient(PlayerGamestate gamestate, IChannel channel, PlayerConnectionInfo pci)
    {
        IsConnected = false;
        IsCompressing = false;
        IsEncrypting = false;
        Gamestate = gamestate;
        Channel = channel;
        PlayerConnectionInfo = pci;
    }

    public Task SendPacket(Packet packet)
    {
        if (!Channel.Active)
            return Task.CompletedTask;

        var buffer = Unpooled.WrappedBuffer(packet.ToArray());
        return Channel.WriteAndFlushAsync(buffer);
    }

    public void DisconnectChannel()
    {
        StopTimers();
        Channel.CloseAsync();
    }
}