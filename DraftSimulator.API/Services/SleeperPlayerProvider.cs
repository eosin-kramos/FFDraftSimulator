using System.Text.Json;

namespace DraftSimulator.API.Services;

public class SleeperPlayerProvider : IPlayerDataProvider
{
    private const string Url = "https://api.sleeper.app/v1/players/nfl";
    private readonly HttpClient _http;

    public SleeperPlayerProvider(HttpClient http) => _http = http;

    // SERVICE method, no Http attributes, no Ok/NotFound
    public async Task<Dictionary<string, JsonElement>> GetActivePlayersAsync()
    {
        using var stream = await _http.GetStreamAsync(Url);
        var raw = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(stream);

        if (raw is null) return new();   // empty dict

        return raw
            .Where(kv =>
                kv.Value.TryGetProperty("active", out var act) && act.GetBoolean() &&
                kv.Value.TryGetProperty("full_name", out var name) && !name.GetString()!.Equals(""))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
