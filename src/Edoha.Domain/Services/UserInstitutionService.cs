using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserInstitution;

namespace Edoha.Domain.Services
{
    public class UserInstitutionService : Service<UserInstitution>, IUserInstitutionService
    {

        public UserInstitutionService(IUserInstitutionRepository repository,
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertUserInstitution(CreateUserInstitutionDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<UserInstitution> SelectUserInstitutionById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserInstitution>> SelectAllUserInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserInstitutionById(UpdateUserInstitutionDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteUserInstitutionById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
