namespace Edoha.Domain.Models.Requests
{
    public class AuthResponse
    {
        public Guid IdUser { get; set; }
        public string AccessToken { get; set; }
    }
}
