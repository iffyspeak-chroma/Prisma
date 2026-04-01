using System.Text.Json;

namespace API.DataPacks;

public class RootDto : Dictionary<string, RegistryDto> {}

public class RegistryDto
{
    public Dictionary<string, JsonElement> Entries { get; set; } = new();
}