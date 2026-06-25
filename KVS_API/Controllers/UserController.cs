using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace KVS_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound("User nicht gefunden");

            return Ok(new UserResponse(user.Id, user.Username, user.Email));
        }




    }
}
