using Edoha.Domain.Models.DTOs.Auth;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IAuthService
    {
        Task<string> Autenticate(CredentialsDTO credentials);
    }
}
