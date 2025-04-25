namespace Edoha.Domain.Entities
{
    public class Institution: Entity
    {
        public string Name{ get; set; } 
        public ICollection<UserInstitution> UserInstitutions { get; set; }

    }
}
