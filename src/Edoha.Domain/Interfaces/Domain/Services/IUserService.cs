using Edoha.Domain.Entities;
using Edoha.Domain.Models.InputModels.User;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.Responses.User;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IUserService : IService<User>
    {
        public Task InsertUser(CreateUserInputModel model);

        Task<Guid> InsertUserInformation(string? name, string? phone);

        public Task<User> SelectUserById(Guid id);

        public Task<IEnumerable<User>> SelectAllUsers();

        public Task<IEnumerable<UserInformationResponse?>> GetUserInformation(bool withTicketbooks);

        public Task UpdateUserById(UpdateUserDTO dto);

        public Task DeleteUserById(Guid id);

        Task<User> SelectUserCredentialsByNickname(string nickname);

        Task<User> ValidateUserCredentials(string nickname, string password);
    }
}