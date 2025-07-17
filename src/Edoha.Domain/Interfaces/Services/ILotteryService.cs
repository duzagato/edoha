using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Lottery;

namespace Edoha.Domain.Interfaces.Services
{
    public interface ILotteryService : IService<Lottery>
    {
        public Task InsertLottery(CreateLotteryDTO dto);

        public Task<Lottery> SelectLotteryById(Guid id);

        public Task<IEnumerable<Lottery>> SelectAllLotteries();

        public Task UpdateLotteryById(UpdateLotteryDTO dto);

        public Task DeleteLotteryById(Guid id);
    }
}
