namespace API.DataTypes.Player;

public class ClientSettings
{
    // https://minecraft.wiki/w/Java_Edition_protocol/Packets#Client_Information_(configuration)
    
    public string Locale { get; set; }
    public byte ViewDistance { get; set; }
    public ClientChatMode ChatMode { get; set; }
    public bool UseColors { get; set; }
    public SkinParts DisplayedSkinPieces { get; set; }
    public DominantHand DominantHand { get; set; }
    public bool EnableTextFiltering { get; set; } 
    public bool ShowInServerList { get; set; }
    public ParticlePreference ParticlePreference { get; set; }
    
    public ClientSettings() {}
}