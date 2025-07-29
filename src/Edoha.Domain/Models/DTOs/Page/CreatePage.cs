using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.DTOs.Page
{
    public class CreatePage
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
