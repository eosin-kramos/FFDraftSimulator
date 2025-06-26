using DraftSimulator.API.Models;

namespace DraftSimulator.API.Services;
public interface IPlayerDataProvider
{
    Task<IEnumerable<Player>> GetPlayersAsync();
}
