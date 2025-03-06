using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.DTOs.Lottery;

namespace Edoha.Domain.Services
{
    public interface ILotteryService
    {
        void CreateLottery(CreateLotteryDTO dto);
    }
}
