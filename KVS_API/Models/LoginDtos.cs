using System.ComponentModel.DataAnnotations;

namespace KVS_API.Models
{
    public record LoginRequest(
        [Required][EmailAddress] string Email,
        [Required] string Password);

    public record RegisterRequest(
        [Required][MinLength(3)] string Username,
        [Required][EmailAddress] string Email,
        [Required][MinLength(8)] string Password);
}