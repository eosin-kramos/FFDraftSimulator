using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DraftSimulator.API.Services;

namespace DraftSimulator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerDataProvider _provider;

    public PlayersController(IPlayerDataProvider provider) => _provider = provider;

    // GET /api/players
    [HttpGet]
    public async Task<IActionResult> GetActivePlayers()
    {
        Dictionary<string, JsonElement> players = await _provider.GetActivePlayersAsync();

        if (players.Count == 0) return NotFound();

        return Ok(players);   // returns full Sleeper JSON for active players
    }
}