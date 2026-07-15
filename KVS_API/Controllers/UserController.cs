using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

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

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (usernameExists) return Conflict(new { field = "username", message = "This username already exists" });

            var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (emailExists) return Conflict(new { field = "email", message = "This email already exists" });

            string passwortHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User(request.Username, request.Email, passwortHash);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = GeneriereJwtToken(newUser);

            return Ok(new
            {
                Token = token,
                User = new UserResponse(newUser.Id, newUser.Username, newUser.Email)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
            {
                return Unauthorized(new { message = "Wrong email or password" });
            }

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

            if (user == null) return NotFound("User not found");

            return Ok(new UserResponse(user.Id, user.Username, user.Email));
        }


        private string GeneriereJwtToken(User user)
        {
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
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
