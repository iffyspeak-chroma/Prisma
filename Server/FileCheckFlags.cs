namespace Server;

[Flags]
public enum FileCheckFlags : byte
{
    Nothing = 0x00,
    PacketReport = 0x01,
    VersionManifest = 0x02,
    VersionData = 0x04,
    DataGenerationFailure = 0x08,
    InformationExtractionFailure = 0x10,
    DownloadFailure = 0x20
}