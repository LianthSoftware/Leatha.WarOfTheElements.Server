using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.Services;
using Leatha.WarOfTheElements.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Leatha.WarOfTheElements.Server.Controllers
{
    [ApiController]
    [Route("api/game-data")]
    public sealed class GameDataController : ControllerBase
    {
        public GameDataController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        private readonly ITemplateService _templateService;

        [HttpPost("templates/create")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTemplatesAsync()
        {
            await _templateService.CreateTemplatesAsync();
            return Ok();
        }

        [HttpGet("templates/spell_template")]
        [ProducesResponseType(typeof(List<SpellInfoObject>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpellTemplatesAsync()
        {
            var response = await _templateService.GetSpellTemplatesAsync();
            var items = response.Select(i => i.AsTransferObject()).ToList();
            return Ok(items);
        }

        [HttpGet("templates/aura_template")]
        [ProducesResponseType(typeof(List<AuraInfoObject>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuraTemplatesAsync()
        {
            var response = await _templateService.GetAuraTemplatesAsync();
            var items = response.Select(i => i.AsTransferObject()).ToList();
            return Ok(items);
        }

        [HttpGet("templates/map_template")]
        [ProducesResponseType(typeof(List<MapInfoObject>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMapTemplatesAsync()
        {
            var response = await _templateService.GetMapTemplatesAsync();
            var items = response.Select(i => i.AsTransferObject()).ToList();
            return Ok(items);
        }
    }
}
