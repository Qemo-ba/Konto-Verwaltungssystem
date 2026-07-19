using System.ComponentModel.DataAnnotations;

namespace KVS_API.Models
{
    public record LoginRequest(
        [property: Required][property: EmailAddress] string Email,
        [property: Required] string Password);

    public record RegisterRequest(
        [property: Required][property: MinLength(3)] string Username,
        [property: Required][property: EmailAddress] string Email,
        [property: Required][property: MinLength(8)] string Password);
}