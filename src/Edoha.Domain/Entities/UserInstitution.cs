namespace Edoha.Domain.Entities
{
    public class UserInstitution
    {
        public int IdUser { get; set; } // id_user
        public User  User { get; set; }
        public int IdInstitution { get; set; } // id_institution
        public Institution Institution { get; set; }
    }
}
