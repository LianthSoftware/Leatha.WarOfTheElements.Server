using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.Services;
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

        //[HttpGet("templates/cards")]
        //[ProducesResponseType(typeof(List<CardTemplateObject>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetCardTemplatesAsync()
        //{
        //    var response = await _templateService.GetCardTemplatesAsync();
        //    var items = response.Select(i => i.AsTransferObject()).ToList();
        //    return Ok(items);
        //}
    }
}
