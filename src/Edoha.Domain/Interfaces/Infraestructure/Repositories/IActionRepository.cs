using Action = Edoha.Domain.Entities.Action;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface IActionRepository : IBaseRepository<Action>
    {
        Task<IEnumerable<Action>> SelectUserActions(Guid idUser, string pageName);
        Task<Action?> SelectUserActionByName(Guid idUser, string pageName, string actionName);
    }

}
