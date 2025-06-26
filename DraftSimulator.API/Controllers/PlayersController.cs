using Microsoft.AspNetCore.Mvc;
using DraftSimulator.API.Models;
using DraftSimulator.API.Services;

namespace DraftSimulator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerDataProvider _provider;
        public PlayersController(IPlayerDataProvider provider) => _provider = provider;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            var players = await _provider.GetPlayersAsync();
            return Ok(players);
        }
    }
}