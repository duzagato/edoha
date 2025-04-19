using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.Lottery;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class LotteryService : Service<Lottery>, ILotteryService
    {

        public LotteryService(ILotteryRepository repository) : base(repository) { }

        public async Task InsertLottery(CreateLotteryRequest request)
        {
            await this.Insert(request);
        }

        public async Task<Lottery> SelectLotteryById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Lottery>> SelectAllLotteries()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateLotteryById(UpdateLotteryRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteLotteryById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
