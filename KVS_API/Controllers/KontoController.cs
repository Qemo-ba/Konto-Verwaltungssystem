using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;

namespace KVS_API.Controllers
{
    [ApiController]
    [Route("api/konto")]
    public class KontoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public KontoController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet("erstelle-user")]
        public IActionResult ErstelleUser();
    }
}
