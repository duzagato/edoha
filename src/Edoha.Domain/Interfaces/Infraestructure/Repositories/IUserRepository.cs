using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.Responses.User;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> SelectUserCredentialsByNickname(string nickname);
        Task<IEnumerable<UserInformationResponse?>> SelectUserInformation(bool withTicketbooks);
    }
}
