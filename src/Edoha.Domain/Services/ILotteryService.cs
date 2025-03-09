using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.DTOs.Lottery;

namespace Edoha.Domain.Services
{
    public interface ILotteryService
    {
        void CreateLottery(CreateLotteryDTO dto);
        Task UpdateLotteryById(UpdateLotteryDTO dto);
        Task DeleteById(int id);
        Task<Lottery> SelectById(int id);
        Task<IEnumerable<Lottery>> SelectAll();
    }
}
