namespace Edoha.Domain.Interfaces.Infraestructure.Services
{
    public interface ISecretsManagerService
    {
        Task<string> GetConnectionStringAsync();
        Task<string> GetJwtKeyAsync();
    }
}
