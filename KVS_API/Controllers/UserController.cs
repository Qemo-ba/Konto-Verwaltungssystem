using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (emailExists) return BadRequest("Diese E-Mail-Adresse wird bereits verwendet.");

            string passwortHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User(request.Username, request.Email, passwortHash);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return Unauthorized("Falsche E-Mail oder falsches Passwort.");

            bool passwortKorrekt = BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash);

            if (!passwortKorrekt) return Unauthorized("Falsche E-Mail oder falsches Passwort.");

            var token = GeneriereJwtToken(user);

            return Ok(new
            {
                Token = token,
                User = new UserResponse(user.Id, user.Username, user.Email)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound("User nicht gefunden");

            return Ok(new UserResponse(user.Id, user.Username, user.Email));
        }


        private string GeneriereJwtToken(User user)
        {
            // Holt den geheimen Schlüssel aus der appsettings.json
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Token gilt 2 Stunden
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
