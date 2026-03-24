namespace API.Player.State;

public enum PlayerGamestate
{
    Status = 1,
    Login = 2,
    Transfer = 3,
    Configuration = 255,
    Play = 254,
    Handshake = 253
}