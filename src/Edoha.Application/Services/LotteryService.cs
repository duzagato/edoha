using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.Lottery;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Services
{
    public class LotteryService : Service<Lottery>
    {

        public LotteryService(ILotteryRepository repository) : base(repository) { }

        public async Task InsertLottery(CreateLotteryDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<Lottery> SelectLotteryById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Lottery>> SelectAllLotteries()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateLotteryById(UpdateLotteryDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteLotteryById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
