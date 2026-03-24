namespace API.Player.Settings;

public enum ClientChatMode
{
    // Reference https://minecraft.wiki/w/Java_Edition_protocol/Chat#Client_chat_mode
    
    Enabled = 0, // See's all messages
    Commands = 1, // See's system messages such as gamemode changes 
    Hidden = 3 // Only see's gameplay crucial overlay messages
}