using Edoha.Domain.Models.DTOs.Action;
using Action = Edoha.Domain.Entities.Action;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IActionService : IService<Action>
    {
        Task InsertAction(CreateActionDTO dto);
        Task<Action> SelectActionById(Guid id);
        Task<IEnumerable<Action>> SelectAllActions();
        Task UpdateActionById(UpdateActionDTO dto);
        Task DeleteActionById(Guid id);
    }
}
