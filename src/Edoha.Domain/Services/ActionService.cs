using Edoha.Domain.Models.DTOs.Action;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;
using Action = Edoha.Domain.Entities.Action;

namespace Edoha.Domain.Services
{
    public class ActionService : Service<Action>, IActionService
    {
        public ActionService(IActionRepository repository,
            IRequestValidationContext requestValidationContext)
            : base(repository, requestValidationContext) { }

        public async Task InsertAction(CreateActionDTO dto)
        {
            await Insert(dto);
        }

        public async Task<Action?> SelectActionById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Action>> SelectAllActions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateActionById(UpdateActionDTO dto)
        {
            await Update(dto);
        }

        public async Task DeleteActionById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
