using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class PageRepository : BaseRepository<Page>, IPageRepository
    {
        public PageRepository(IDbConnection connection) : base(connection)
        {

        }
    }
}
