using Edoha.Domain.Models.DTOs.Auth;
using Edoha.Domain.Models.Requests;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Autenticate(CredentialsDTO credentials);
    }
}
