using Edoha.Domain.Entities;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface IInstitutionRepository : IBaseRepository<Institution>
    {
        public Task<Institution?> SelectInstitutionBySlug(string slug);
        public Task<IEnumerable<Institution>> SelectInstitutionsByUser(Guid idUser);
    }
}
