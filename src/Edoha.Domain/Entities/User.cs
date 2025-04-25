namespace Edoha.Domain.Entities
{
    public  class User
    {
        public string Name { get; set; } // name_user
        public string Phone { get; set; } // phone_user
        public string Nickname { get; set; } // nickname_user
        public byte[] Password { get; set; } // password_user (varbinary)
        public int IdUserType { get; set; } // id_user_type
        public ICollection<UserInstitution> UserInstitutions { get; set; }

    }
}
