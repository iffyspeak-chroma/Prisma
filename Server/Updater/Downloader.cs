namespace Server.Updater;

public class Downloader
{
    public static async Task DownloadFileAsync(HttpClient http, string url, string output)
    {
        using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None);

        await stream.CopyToAsync(fileStream);
    }
}