using System.Security.Cryptography;

namespace Server;

public class HashUtil
{
    public static async Task<string> ComputeSha1Async(string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        using var sha1 = SHA1.Create();

        var hash = await sha1.ComputeHashAsync(stream);
        
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}