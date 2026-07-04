using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;
using KVS_API.Services;
using Microsoft.AspNetCore.Authorization;

namespace KVS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/konto")]
    public class KontoController : ControllerBase
    {
        private readonly IKontoService _kontoService;

        public KontoController(IKontoService kontoService)
        {
            _kontoService = kontoService;
        }

        [HttpGet("getall/{userId:guid}")]
        public async Task<IActionResult> GetAlleKonten(Guid userId)
        {
            var konten = await _kontoService.GetAlleKontenAsync(userId);
            return Ok(konten);
        }

        [HttpGet("{kontonummer}")]
        public async Task<IActionResult> GetKonto(string kontonummer)
        {
            var konto = await _kontoService.GetKontoAsync(kontonummer);
            return Ok(konto);
        }

        [HttpPost("sparkonto/{userId:guid}")]
        public async Task<IActionResult> ErstelleSparkonto(Guid userId)
        {
            var konto = await _kontoService.ErstelleSparkontoAsync(userId);
            return Ok(konto);
        }

        [HttpPost("privatkonto/{userId:guid}")]
        public async Task<IActionResult> ErstellePrivatkonto(Guid userId)
        {
            var konto = await _kontoService.ErstellePrivatkontoAsync(userId);
            return Ok(konto);
        }

        [HttpPost("{kontonummer}/einzahlen")]
        public async Task<IActionResult> Einzahlen(string kontonummer, [FromBody] EinzahlenRequest request)
        {
            var response = await _kontoService.EinzahlenAsync(kontonummer, request.Betrag);
            return Ok(response);
        }

        [HttpPost("{kontonummer}/auszahlen")]
        public async Task<IActionResult> Auszahlen(string kontonummer, [FromBody] AuszahlenRequest request)
        {
            var response = await _kontoService.AuszahlenAsync(kontonummer, request.Betrag);
            return Ok(response);
        }

        [HttpPost("umbuchen")]
        public async Task<IActionResult> Umbuchen([FromBody] UmbuchenRequest request)
        {
            var response = await _kontoService.UmbuchenAsync(request.VonKontonummer, request.NachKontonummer, request.Betrag);
            return Ok(response);
        }



    }
}
