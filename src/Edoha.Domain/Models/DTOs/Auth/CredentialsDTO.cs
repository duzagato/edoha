using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.DTOs.Auth
{
    public class CredentialsDTO : DTO
    {
        [Required]
        public string? Nickname { get; set; } 

        [Required]
        public string? Password { get; set; }
    }
}
