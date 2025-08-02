using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infrastructure.Repositories;
using Action = Edoha.Domain.Entities.Action;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class ActionRepository : BaseRepository<Action>, IActionRepository
    {
        public ActionRepository(IDbConnection connection) : base(connection)
        {

        }
    }
}
