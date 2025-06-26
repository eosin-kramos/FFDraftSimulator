using System.Text.Json;
using DraftSimulator.API.Models;

namespace DraftSimulator.API.Services;

public class SleeperPlayerProvider : IPlayerDataProvider
{
    private const string Url = "https://api.sleeper.app/v1/players/nfl";
    private readonly HttpClient _http;
    public SleeperPlayerProvider(HttpClient http) => _http = http;

    public async Task<IEnumerable<Player>> GetPlayersAsync()
    {
        using var stream = await _http.GetStreamAsync(Url);
        var raw = await JsonSerializer
            .DeserializeAsync<Dictionary<string, JsonElement>>(stream);

        if (raw is null) return Enumerable.Empty<Player>();
        return raw.Values                     // each JsonElement is a player
            .Where(p => p.TryGetProperty("active", out var act) && act.GetBoolean()) // keep active
            .Select(p => new Player
            {
                ID = p.TryGetProperty("player_id", out var idProp) && int.TryParse(idProp.GetString(), out var idVal) ? idVal : 0,
                Name = p.TryGetProperty("full_name", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty,
                Position = p.TryGetProperty("position", out var posProp) ? posProp.GetString() ?? string.Empty : string.Empty,
                Team = p.TryGetProperty("team", out var teamProp) ? teamProp.GetString() ?? string.Empty : string.Empty,
                Points = 0
            })
            .ToList();
}
}