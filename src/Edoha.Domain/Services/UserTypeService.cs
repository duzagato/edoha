using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserType;

namespace Edoha.Domain.Services
{
    public class UserTypeService : Service<UserType>, IUserTypeService
    {

        public UserTypeService(IUserTypeRepository repository,
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertUserType(CreateUserTypeDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<UserType> SelectUserTypeById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserType>> SelectAllUserTypes()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserTypeById(UpdateUserTypeDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteUserTypeById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
