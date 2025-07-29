using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Lottery;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public class LotteryService : Service<Lottery>, ILotteryService
    {

        public LotteryService(ILotteryRepository repository, 
            IRequestValidationContext requestValidationContext)
            : base(repository, requestValidationContext) { }

        public async Task InsertLottery(CreateLotteryDTO dto)
        {
            await Insert(dto);
        }

        public async Task<Lottery> SelectLotteryById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Lottery>> SelectAllLotteries()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateLotteryById(UpdateLotteryDTO dto)
        {
            await Update(dto);
        }

        public async Task DeleteLotteryById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
