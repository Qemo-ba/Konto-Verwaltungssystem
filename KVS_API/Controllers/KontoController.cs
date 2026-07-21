using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;
using KVS_API.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KVS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/konto")]
    public class KontoController : ControllerBase
    {
        private readonly IKontoService _kontoService;
        private readonly IKontobewegungService _bewegungService;

        public KontoController(IKontoService kontoService, IKontobewegungService bewegungService)
        {
            _kontoService = kontoService;
            _bewegungService = bewegungService;
        }

        [HttpGet("{kontonummer}/bewegungen")]
        public async Task<IActionResult> GetBewegungen(string kontonummer)
        {
            var bewegungen = await _bewegungService.GetBewegungenAsync(kontonummer, AktuellerUserId());
            return Ok(bewegungen);
        }

        [HttpGet("getkonten")]
        public async Task<IActionResult> GetAlleKonten()
        {
            var konten = await _kontoService.GetAlleKontenAsync(AktuellerUserId());
            return Ok(konten);
        }

        [HttpGet("{kontonummer}")]
        public async Task<IActionResult> GetKonto(string kontonummer)
        {
            var konto = await _kontoService.GetKontoAsync(kontonummer, AktuellerUserId());
            return Ok(konto);
        }

        [HttpPost("sparkonto")]
        public async Task<IActionResult> ErstelleSparkonto()
        {
            var konto = await _kontoService.ErstelleSparkontoAsync(AktuellerUserId());
            return Ok(konto);
        }

        [HttpPost("privatkonto")]
        public async Task<IActionResult> ErstellePrivatkonto()
        {
            var konto = await _kontoService.ErstellePrivatkontoAsync(AktuellerUserId());
            return Ok(konto);
        }

        [HttpPost("{kontonummer}/einzahlen")]
        public async Task<IActionResult> Einzahlen(string kontonummer, [FromBody] EinzahlenRequest request)
        {
            var response = await _kontoService.EinzahlenAsync(kontonummer, request.Betrag, AktuellerUserId());
            return Ok(response);
        }

        [HttpPost("{kontonummer}/auszahlen")]
        public async Task<IActionResult> Auszahlen(string kontonummer, [FromBody] AuszahlenRequest request)
        {
            var response = await _kontoService.AuszahlenAsync(kontonummer, request.Betrag, AktuellerUserId());
            return Ok(response);
        }

        [HttpPost("umbuchen")]
        public async Task<IActionResult> Umbuchen([FromBody] UmbuchenRequest request)
        {
            var response = await _kontoService.UmbuchenAsync(request.VonKontonummer, request.NachKontonummer, request.Betrag, AktuellerUserId());
            return Ok(response);
        }

        [HttpDelete("{kontonummer}")]
        public async Task<IActionResult> KontoEntfernen(string kontonummer)
        {
            await _kontoService.KontoEntfernen(kontonummer, AktuellerUserId());
            return NoContent();
        }

        private Guid AktuellerUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(id!);
        }


    }
}
