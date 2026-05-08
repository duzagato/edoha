using Edoha.Domain.Entities;

namespace Edoha.Domain.Models.Requests
{
    public class AuthResponse
    {
        public Guid IdUser { get; set; }
        public string AccessToken { get; set; }
        public List<Institution> Institutions { get; set; }
    }
}
