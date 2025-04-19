using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.Requests.Lottery;

namespace Edoha.Domain.Interfaces.Services
{
    public interface ILotteryService : IService<Lottery>
    {
        public Task InsertLottery(CreateLotteryRequest request);

        public Task<Lottery> SelectLotteryById(int id);

        public Task<IEnumerable<Lottery>> SelectAllLotteries();

        public Task UpdateLotteryById(UpdateLotteryRequest request);

        public Task DeleteLotteryById(int id);
    }
}
