using System.ComponentModel.DataAnnotations;

namespace KVS_API.Models
{
    public record LoginRequest(string Email, string Password);

    public record RegisterRequest(string Username, string Email, string Password);
}