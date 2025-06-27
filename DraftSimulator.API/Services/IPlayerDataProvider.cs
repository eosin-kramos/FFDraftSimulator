using System.Text.Json;

public interface IPlayerDataProvider
{
    Task<Dictionary<string, JsonElement>> GetActivePlayersAsync();
}