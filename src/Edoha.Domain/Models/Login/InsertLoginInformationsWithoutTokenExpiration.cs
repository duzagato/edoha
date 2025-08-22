using Edoha.Domain.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.Login
{
    public class InsertLoginInformationsWithoutTokenExpiration : DTO
    {
        public Guid IdUser { get; set; }

        [Required]
        public string Ip { get; set; }

        [Required]
        public string RefreshTokenHash { get; set; }
    }
}
