using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.DTOs.Action
{
    public class CreateActionDTO : DTO
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool? WithoutOwner { get; set; }
        public bool? OtherOwner { get; set; }
    }
}
