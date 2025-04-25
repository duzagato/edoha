using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Entities;

public abstract class Entity
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt{ get; set; }
    public DateTime UpdatedAt{ get; set; }
    public string CreatedBy{ get; set; }
    public string UpdatedBy{ get; set; }
}
/* Tudo o que tiver id é relacionamento
 Muda do mysql server pro postgres*/