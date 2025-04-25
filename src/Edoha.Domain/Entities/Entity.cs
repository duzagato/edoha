namespace Edoha.Domain.Entities;

public abstract class Entity
{
    public int Id { get; set; }
    public DateTime CreatedAt{ get; set; }
    public DateTime UpdatedAt{ get; set; }
    public string CreatedBy{ get; set; }
    public string UpdatedBy{ get; set; }
}
/* Tudo o que tiver id é relacionamento */