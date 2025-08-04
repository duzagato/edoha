using Edoha.Domain.Entities;

namespace Edoha.Domain.Interfaces.Infraestructure.Services
{
    public interface ITokenGenerationService
    {
        string GenerateToken(User user, string permissionsJson);
    }
}
